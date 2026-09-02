using Microsoft.EntityFrameworkCore;
using QuebecInsider.Api.Data;
using QuebecInsider.Api.Domain;
using QuebecInsider.Api.Dtos;

namespace QuebecInsider.Api.Services;

public sealed class ArticleService(QuebecInsiderDbContext db) : IArticleService
{
    public async Task<IReadOnlyList<ArticleDto>> GetArticlesAsync(string culture, string? categorySlug, CancellationToken cancellationToken)
    {
        var query = BaseQuery(culture);

        if (!string.IsNullOrWhiteSpace(categorySlug))
        {
            query = query.Where(article => article.Category.Translations.Any(t => t.Culture == culture && t.Slug == categorySlug));
        }

        var articles = await query
            .OrderByDescending(article => article.PublishedAt)
            .ToListAsync(cancellationToken);

        return articles.Select(article => Map(article, culture)).ToList();
    }

    public async Task<IReadOnlyList<ArticleDto>> GetTrendingAsync(string culture, CancellationToken cancellationToken)
    {
        var articles = await BaseQuery(culture)
            .OrderByDescending(article => article.PopularityScore)
            .ThenByDescending(article => article.PublishedAt)
            .Take(5)
            .ToListAsync(cancellationToken);

        return articles.Select(article => Map(article, culture)).ToList();
    }

    public async Task<ArticleDto?> GetArticleAsync(string culture, string slug, CancellationToken cancellationToken)
    {
        var article = await BaseQuery(culture)
            .FirstOrDefaultAsync(article => article.Translations.Any(t => t.Culture == culture && t.Slug == slug), cancellationToken);

        return article is null ? null : Map(article, culture);
    }

    public async Task<IReadOnlyList<ArticleDto>> SearchAsync(string culture, string query, CancellationToken cancellationToken)
    {
        var normalized = query.Trim();
        if (normalized.Length == 0)
        {
            return [];
        }

        var articles = await BaseQuery(culture)
            .Where(article => article.Translations.Any(t =>
                t.Culture == culture &&
                (t.Title.Contains(normalized) ||
                 t.Description.Contains(normalized) ||
                 t.Body.Contains(normalized) ||
                 t.Keywords.Contains(normalized))) ||
                article.Category.Translations.Any(t => t.Culture == culture && t.Name.Contains(normalized)))
            .OrderByDescending(article => article.PopularityScore)
            .ToListAsync(cancellationToken);

        return articles.Select(article => Map(article, culture)).ToList();
    }

    private IQueryable<Article> BaseQuery(string culture) =>
        db.Articles
            .AsNoTracking()
            .Include(article => article.Category).ThenInclude(category => category.Translations)
            .Include(article => article.Translations)
            .Where(article => article.Translations.Any(t => t.Culture == culture));

    private static ArticleDto Map(Article article, string culture)
    {
        var translation = article.Translations.Single(t => t.Culture == culture);
        var categoryTranslation = article.Category.Translations.Single(t => t.Culture == culture);
        var alternateCulture = culture == "fr-CA" ? "en-CA" : "fr-CA";
        var alternateSlug = article.Translations.SingleOrDefault(t => t.Culture == alternateCulture)?.Slug;

        return new ArticleDto(
            article.Id,
            culture,
            translation.Title,
            translation.Slug,
            translation.Description,
            translation.Body,
            translation.SeoTitle,
            translation.SeoDescription,
            translation.Keywords,
            translation.InsiderTip,
            categoryTranslation.Name,
            categoryTranslation.Slug,
            article.ImageUrl,
            article.PublishedAt,
            article.ReadingMinutes,
            article.AuthorName,
            article.PopularityScore,
            alternateSlug);
    }
}
