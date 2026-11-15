using System.Net.Http.Json;
using System.Text.Json;
using QuebecInsider.Api.Dtos;

namespace QuebecInsider.Api.Services;

public interface IElasticsearchService
{
    bool IsEnabled { get; }
    Task IndexArticlesAsync(IEnumerable<ArticleDto> articles, CancellationToken ct = default);
    Task<IReadOnlyList<ArticleDto>?> SearchArticlesAsync(string culture, string query, CancellationToken ct = default);
}

public sealed class ElasticsearchService(
    HttpClient http,
    IConfiguration config,
    ILogger<ElasticsearchService> logger) : IElasticsearchService
{
    private readonly string? _baseUrl = config["Elasticsearch:Url"]?.TrimEnd('/');
    private const string IndexName = "quebecinsider-articles";

    public bool IsEnabled => !string.IsNullOrWhiteSpace(_baseUrl);

    public async Task IndexArticlesAsync(IEnumerable<ArticleDto> articles, CancellationToken ct = default)
    {
        if (!IsEnabled) return;

        try
        {
            foreach (var article in articles)
            {
                var docId = $"{article.Id}-{article.Culture}";
                var response = await http.PutAsJsonAsync(
                    $"{_baseUrl}/{IndexName}/_doc/{docId}",
                    article,
                    ct);
                response.EnsureSuccessStatusCode();
            }

            await http.PostAsync($"{_baseUrl}/{IndexName}/_refresh", null, ct);
            logger.LogInformation("Articles indexés avec succès dans Elasticsearch ({Index}).", IndexName);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch indisponible lors de l'indexation (fallback PostgreSQL actif).");
        }
    }

    public async Task<IReadOnlyList<ArticleDto>?> SearchArticlesAsync(string culture, string query, CancellationToken ct = default)
    {
        if (!IsEnabled) return null;

        try
        {
            var searchPayload = new
            {
                query = new
                {
                    @bool = new
                    {
                        must = new object[]
                        {
                            new
                            {
                                multi_match = new
                                {
                                    query,
                                    fields = new[] { "title^3", "description^2", "keywords^2", "body", "categoryName" },
                                    fuzziness = "AUTO"
                                }
                            }
                        },
                        filter = new object[]
                        {
                            new { term = new { data = new { } } } // remplacé ci-dessous par match culture
                        }
                    }
                }
            };

            var esQuery = new
            {
                size = 20,
                query = new
                {
                    @bool = new
                    {
                        must = new object[]
                        {
                            new
                            {
                                multi_match = new
                                {
                                    query,
                                    fields = new[] { "title^3", "description^2", "keywords^2", "body", "categoryName" },
                                    fuzziness = "AUTO"
                                }
                            },
                            new
                            {
                                match_phrase = new { culture }
                            }
                        }
                    }
                }
            };

            var response = await http.PostAsJsonAsync($"{_baseUrl}/{IndexName}/_search", esQuery, ct);
            if (!response.IsSuccessStatusCode) return null;

            using var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
            var hits = doc.RootElement.GetProperty("hits").GetProperty("hits");

            var results = new List<ArticleDto>();
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            foreach (var hit in hits.EnumerateArray())
            {
                var source = hit.GetProperty("_source");
                var article = source.Deserialize<ArticleDto>(jsonOptions);
                if (article is not null)
                {
                    results.Add(article);
                }
            }

            logger.LogInformation("Recherche Elasticsearch '{Query}' ({Culture}) -> {Count} résultat(s).", query, culture, results.Count);
            return results;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Elasticsearch indisponible pour la recherche '{Query}', bascule sur PostgreSQL.", query);
            return null;
        }
    }
}