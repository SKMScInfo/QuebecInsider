namespace QuebecInsider.Api.Domain;

public sealed class Category
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public List<CategoryTranslation> Translations { get; set; } = [];
    public List<Article> Articles { get; set; } = [];
}
