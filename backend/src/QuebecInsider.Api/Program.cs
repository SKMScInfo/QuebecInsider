using Microsoft.EntityFrameworkCore;
using QuebecInsider.Api.Data;
using QuebecInsider.Api.Dtos;
using QuebecInsider.Api.Hubs;
using QuebecInsider.Api.Services;

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
        options.UseSqlServer(connection);
    }
});

builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalFrontend", policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<QuebecInsiderDbContext>();
    await db.Database.EnsureCreatedAsync();
    await SeedData.InitializeAsync(db);
}

app.UseCors("LocalFrontend");
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api/{culture:regex(^fr-CA|en-CA$)}/articles", async (
    string culture,
    string? category,
    IArticleService articles,
    CancellationToken cancellationToken) =>
    await articles.GetArticlesAsync(culture, category, cancellationToken))
    .WithName("GetArticles");

app.MapGet("/api/{culture:regex(^fr-CA|en-CA$)}/articles/trending", async (
    string culture,
    IArticleService articles,
    CancellationToken cancellationToken) =>
    await articles.GetTrendingAsync(culture, cancellationToken))
    .WithName("GetTrendingArticles");

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
    .WithName("GetCategories");

app.MapPost("/api/newsletter", (NewsletterSignupDto signup) =>
{
    if (string.IsNullOrWhiteSpace(signup.Email))
    {
        return Results.BadRequest(new { message = "Email is required." });
    }

    return Results.Accepted(value: new { message = "Signup received.", signup.Email, signup.Culture });
})
    .WithName("NewsletterSignup");

app.MapHub<TrendingHub>("/hubs/trending");

app.Run();

public partial class Program;
