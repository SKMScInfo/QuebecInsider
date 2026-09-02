using QuebecInsider.Api.Dtos;

namespace QuebecInsider.Api.Services;

public interface IArticleService
{
    Task<IReadOnlyList<ArticleDto>> GetArticlesAsync(string culture, string? categorySlug, CancellationToken cancellationToken);
    Task<IReadOnlyList<ArticleDto>> GetTrendingAsync(string culture, CancellationToken cancellationToken);
    Task<ArticleDto?> GetArticleAsync(string culture, string slug, CancellationToken cancellationToken);
    Task<IReadOnlyList<ArticleDto>> SearchAsync(string culture, string query, CancellationToken cancellationToken);
}
