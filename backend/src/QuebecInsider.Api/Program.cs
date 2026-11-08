using Microsoft.EntityFrameworkCore;
using QuebecInsider.Api.Data;
using QuebecInsider.Api.Dtos;
using QuebecInsider.Api.Hubs;
using QuebecInsider.Api.Services;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using MassTransit;
using QuebecInsider.Api.Events;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.OutputCaching;
using FluentValidation;
using QuebecInsider.Api.Domain;
using QuebecInsider.Api.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<QuebecInsiderDbContext>(options =>
{
    var connection = builder.Configuration.GetConnectionString("QuebecInsider");
    if (string.IsNullOrWhiteSpace(connection))
    {
        options.UseInMemoryDatabase("QuebecInsiderLocal");
    }
    else
    {
        options.UseNpgsql(connection);
    }
});

builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ISnowflakeAnalyticsService, SnowflakeAnalyticsService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalFrontend", policy =>
        policy.SetIsOriginAllowed(origin =>
                Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                (uri.Host == "localhost" ||
                 uri.Host == "quebecinsider.ca" ||
                 uri.Host.EndsWith(".quebecinsider.ca", StringComparison.OrdinalIgnoreCase) ||
                 uri.Host.EndsWith(".vercel.app", StringComparison.OrdinalIgnoreCase) ||
                 uri.Host.EndsWith(".azurecontainerapps.io", StringComparison.OrdinalIgnoreCase)))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
var jwtKey = builder.Configuration["Jwt:Key"] ?? "QuebecInsiderSuperSecretKey2026!EnterpriseGrade";
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "QuebecInsider.Api",
            ValidAudience = "QuebecInsider.Web",
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<QuebecInsiderMetrics>();
builder.Services.AddMassTransit(bus =>
{
    bus.AddConsumer<ArticleViewedConsumer>();

    var rabbitUrl = builder.Configuration["RabbitMQ:Url"];
    var rabbitHost = builder.Configuration["RabbitMQ:Host"];

    if (!string.IsNullOrWhiteSpace(rabbitUrl))
    {
        bus.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(new Uri(rabbitUrl));
            cfg.ConfigureEndpoints(context);
        });
    }
    else if (!string.IsNullOrWhiteSpace(rabbitHost))
    {
        bus.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(rabbitHost, "/", h =>
            {
                h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
                h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
            });
            cfg.ConfigureEndpoints(context);
        });
    }
    else
    {
        bus.UsingInMemory((context, cfg) =>
        {
            cfg.ConfigureEndpoints(context);
        });
    }
});
// Standard RFC 9457 pour les erreurs HTTP JSON propres
builder.Services.AddProblemDetails();

builder.Services.AddValidatorsFromAssemblyContaining<NewsletterSignupValidator>();

// Si Redis est configuré (Docker local ou Upstash Redis Cloud), OutputCache stocke dans Redis !
// Sinon (sur Azure gratuit sans Redis), il bascule automatiquement en RAM à 0,00 $.
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrWhiteSpace(redisConnection))
{
    builder.Services.AddStackExchangeRedisOutputCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "QuebecInsider:";
    });
}
// OutputCache (.NET 10) avec tags d'invalidation
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("ArticlesCache", p => p.Expire(TimeSpan.FromSeconds(30)).Tag("articles"));
    options.AddPolicy("TrendingCache", p => p.Expire(TimeSpan.FromSeconds(15)).Tag("trending"));
    options.AddPolicy("CategoriesCache", p => p.Expire(TimeSpan.FromMinutes(5)).Tag("categories"));
});

// Rate Limiting (.NET 10) : max 60 requêtes / minute par IP
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("ApiLimit", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

// Sondes de santé Liveness & Readiness (vérifie la connexion PostgreSQL)
builder.Services.AddHealthChecks()
    .AddDbContextCheck<QuebecInsiderDbContext>(name: "database", tags: ["ready"]);

// 1. Configurer OpenTelemetry
var otel = builder.Services.AddOpenTelemetry();
otel.ConfigureResource(resource => resource
    .AddService(serviceName: "QuebecInsider.Api", serviceVersion: "1.0.0"));
// 2. Métriques (Prometheus + Compteurs personnalisés)
otel.WithMetrics(metrics =>
{
    metrics
        .AddAspNetCoreInstrumentation() // Requêtes HTTP, latence, codes 200/404/500
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()    // CPU, GC, mémoire .NET
        .AddMeter("QuebecInsider.Metrics") // Ton compteur métier custom (voir étape 4)
        .AddPrometheusExporter();       // Active l'export au format Prometheus
});
// 3. Traces distribuées (HTTP -> ArticleService -> EF Core SQL)
otel.WithTracing(tracing =>
{
    tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation() // Trace les requêtes SQL de QuebecInsiderDbContext
        .AddSource("QuebecInsider.Tracing");
});
// 4. Brancher Azure Monitor si la Connection String est présente dans appsettings / variables d'env
var azureMonitorConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
if (!string.IsNullOrWhiteSpace(azureMonitorConnectionString))
{
    otel.UseAzureMonitor(options =>
    {
        options.ConnectionString = azureMonitorConnectionString;
    });
}

var app = builder.Build();

// Expose http://localhost:5295/metrics pour Prometheus
app.MapPrometheusScrapingEndpoint();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuebecInsiderDbContext>();
    await db.Database.EnsureCreatedAsync();
    if (db.Database.IsRelational())
    {
        await db.Database.ExecuteSqlRawAsync("""
            CREATE TABLE IF NOT EXISTS "NewsletterSubscribers" (
                "Id" SERIAL PRIMARY KEY,
                "Email" TEXT NOT NULL UNIQUE,
                "Culture" TEXT NOT NULL,
                "SubscribedAtUtc" TIMESTAMPTZ NOT NULL
            );
            """);
    }
    await SeedData.InitializeAsync(db);
}

app.UseCors("LocalFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseRateLimiter();
app.UseOutputCache();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/{culture:regex(^fr-CA|en-CA$)}/articles", async (
    string culture,
    string? category,
    IArticleService articles,
    CancellationToken cancellationToken) =>
    await articles.GetArticlesAsync(culture, category, cancellationToken))
    .WithName("GetArticles")
    .CacheOutput("ArticlesCache")
    .RequireRateLimiting("ApiLimit");

app.MapGet("/api/{culture:regex(^fr-CA|en-CA$)}/articles/trending", async (
    string culture,
    IArticleService articles,
    CancellationToken cancellationToken) =>
    await articles.GetTrendingAsync(culture, cancellationToken))
    .WithName("GetTrendingArticles")
    .CacheOutput("TrendingCache")
    .RequireRateLimiting("ApiLimit");

app.MapGet("/api/{culture:regex(^fr-CA|en-CA$)}/articles/{slug}", async (
    string culture,
    string slug,
    IArticleService articles,
    CancellationToken cancellationToken) =>
{
    var article = await articles.GetArticleAsync(culture, slug, cancellationToken);
    return article is null ? Results.NotFound() : Results.Ok(article);
})
    .WithName("GetArticle");

app.MapGet("/api/{culture:regex(^fr-CA|en-CA$)}/search", async (
    string culture,
    string q,
    IArticleService articles,
    CancellationToken cancellationToken) =>
    await articles.SearchAsync(culture, q, cancellationToken))
    .WithName("SearchArticles");

app.MapGet("/api/{culture:regex(^fr-CA|en-CA$)}/categories", async (
    string culture,
    QuebecInsiderDbContext db,
    CancellationToken cancellationToken) =>
    await db.Categories
        .AsNoTracking()
        .Select(category => new CategoryDto(
            category.Key,
            category.Translations.First(t => t.Culture == culture).Name,
            category.Translations.First(t => t.Culture == culture).Slug,
            category.Translations.First(t => t.Culture == culture).Description))
        .ToListAsync(cancellationToken))
    .WithName("GetCategories")
    .CacheOutput("CategoriesCache")
    .RequireRateLimiting("ApiLimit");

app.MapPost("/api/newsletter", async (
    NewsletterSignupDto signup,
    IValidator<NewsletterSignupDto> validator,
    QuebecInsiderDbContext db,
    CancellationToken ct) =>
{
    var validation = await validator.ValidateAsync(signup, ct);
    if (!validation.IsValid)
    {
        return Results.ValidationProblem(validation.ToDictionary());
    }

    var normalizedEmail = signup.Email.Trim().ToLowerInvariant();
    var exists = await db.NewsletterSubscribers
        .AnyAsync(s => s.Email == normalizedEmail, ct);

    if (!exists)
    {
        db.NewsletterSubscribers.Add(new NewsletterSubscriber
        {
            Email = normalizedEmail,
            Culture = signup.Culture,
            SubscribedAtUtc = DateTimeOffset.UtcNow
        });
        await db.SaveChangesAsync(ct);
    }

    return Results.Accepted(value: new { message = "Signup received.", Email = normalizedEmail, signup.Culture });
})
.WithName("NewsletterSignup")
.RequireRateLimiting("ApiLimit");

// 1. Générer un jeton JWT Admin (Username: "admin", Password: "Password123!")
app.MapPost("/api/auth/token", (LoginRequestDto login, IConfiguration config) =>
{
    var adminPass = config["Jwt:AdminPassword"] ?? "Password123!";
    if (login.Username != "admin" || login.Password != adminPass)
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, login.Username),
        new Claim(ClaimTypes.Name, login.Username),
        new Claim(ClaimTypes.Role, "Admin")
    };

    var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    var token = new JwtSecurityToken(
        issuer: "QuebecInsider.Api",
        audience: "QuebecInsider.Web",
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: creds);

    return Results.Ok(new
    {
        accessToken = new JwtSecurityTokenHandler().WriteToken(token),
        expiresIn = 7200
    });
})
.WithName("GenerateAdminToken");

// 2. Route protégée par JWT (Rôle Admin requis -> renvoie 401/403 sans jeton !)
app.MapPost("/api/admin/cache/evict", async (IOutputCacheStore cacheStore, CancellationToken ct) =>
{
    await cacheStore.EvictByTagAsync("articles", ct);
    await cacheStore.EvictByTagAsync("trending", ct);
    await cacheStore.EvictByTagAsync("categories", ct);
    return Results.Ok(new { message = "Tous les caches Redis ont été purgés par l'administrateur." });
})
.WithName("EvictAllCaches")
.RequireAuthorization("AdminOnly");

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // Vérifie uniquement que le processus .NET répond
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready") // Vérifie que PostgreSQL répond
});
app.MapHub<TrendingHub>("/hubs/trending");

app.Run();

public partial class Program;
