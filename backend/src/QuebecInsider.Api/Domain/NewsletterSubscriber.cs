namespace QuebecInsider.Api.Domain;

public sealed class NewsletterSubscriber
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Culture { get; set; } = "fr-CA";
    public DateTimeOffset SubscribedAtUtc { get; set; }
}