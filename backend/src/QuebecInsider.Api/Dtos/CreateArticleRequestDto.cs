namespace QuebecInsider.Api.Dtos;

public sealed record CreateArticleTranslationDto(
    string Title,
    string Slug,
    string Description,
    string Body,
    string SeoTitle,
    string SeoDescription,
    string Keywords,
    string InsiderTip
);

public sealed record CreateArticleRequestDto(
    string CategoryKey, // ex: "money", "deals", "food", "going-out", "travel", "tech", "life"
    string ImageUrl,
    string AuthorName,
    int ReadingMinutes,
    int InitialPopularityScore,
    CreateArticleTranslationDto French,
    CreateArticleTranslationDto English
);