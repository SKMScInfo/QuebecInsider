namespace QuebecInsider.Api.Domain;

public sealed class Article
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public string ImageUrl { get; set; } = string.Empty;
    public DateTimeOffset PublishedAt { get; set; }
    public int ReadingMinutes { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int PopularityScore { get; set; }
    public List<ArticleTranslation> Translations { get; set; } = [];
}
