namespace QuebecInsider.Api.Events;

public sealed record ArticleViewedEvent(
    int ArticleId,
    string Culture,
    string Slug,
    DateTimeOffset ViewedAtUtc);