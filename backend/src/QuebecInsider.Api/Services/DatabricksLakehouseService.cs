using System.Net.Http.Headers;
using System.Net.Http.Json;
using QuebecInsider.Api.Events;

namespace QuebecInsider.Api.Services;

public interface IDatabricksLakehouseService
{
    bool IsEnabled { get; }
    Task IngestBronzeEventAsync(ArticleViewedEvent evt, CancellationToken ct = default);
}

public sealed class DatabricksLakehouseService(
    HttpClient http,
    IConfiguration config,
    ILogger<DatabricksLakehouseService> logger) : IDatabricksLakehouseService
{
    // Ex: "https://adb-123456789.12.azuredatabricks.net" ou "https://dbc-xxxx.cloud.databricks.com"
    private readonly string? _workspaceUrl = config["Databricks:WorkspaceUrl"]?.TrimEnd('/');
    private readonly string? _accessToken = config["Databricks:AccessToken"];
    private readonly string? _warehouseId = config["Databricks:WarehouseId"];

    public bool IsEnabled =>
        !string.IsNullOrWhiteSpace(_workspaceUrl) &&
        !string.IsNullOrWhiteSpace(_accessToken) &&
        !string.IsNullOrWhiteSpace(_warehouseId);

    public async Task IngestBronzeEventAsync(ArticleViewedEvent evt, CancellationToken ct = default)
    {
        if (!IsEnabled) return;

        try
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"{_workspaceUrl}/api/2.0/sql/statements");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);

            var statement = """
                INSERT INTO  workspace.default.bronze_article_views (article_id, culture, slug, viewed_at_utc)
                VALUES (:article_id, :culture, :slug, :viewed_at_utc)
                """;

            request.Content = JsonContent.Create(new
            {
                warehouse_id = _warehouseId,
                statement,
                wait_timeout = "10s",
                parameters = new object[]
                {
                    new { name = "article_id", value = evt.ArticleId.ToString(), type = "INT" },
                    new { name = "culture", value = evt.Culture, type = "STRING" },
                    new { name = "slug", value = evt.Slug, type = "STRING" },
                    new { name = "viewed_at_utc", value = evt.ViewedAtUtc.ToString("O"), type = "TIMESTAMP" }
                }
            });

            var response = await http.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            logger.LogInformation(
                "Événement ingéré dans Databricks Delta Lake (bronze_article_views) : Article #{Id} ({Slug})",
                evt.ArticleId, evt.Slug);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Databricks Lakehouse indisponible ou non configuré (non-bloquant, Snowflake actif).");
        }
    }
}