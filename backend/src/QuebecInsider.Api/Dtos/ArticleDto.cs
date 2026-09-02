namespace QuebecInsider.Api.Dtos;

public sealed record ArticleDto(
    int Id,
    string Culture,
    string Title,
    string Slug,
    string Description,
    string Body,
    string SeoTitle,
    string SeoDescription,
    string Keywords,
    string InsiderTip,
    string CategoryName,
    string CategorySlug,
    string ImageUrl,
    DateTimeOffset PublishedAt,
    int ReadingMinutes,
    string AuthorName,
    int PopularityScore,
    string? AlternateCultureSlug);
