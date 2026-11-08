using System.Diagnostics.Metrics;

namespace QuebecInsider.Api.Services;

public sealed class QuebecInsiderMetrics
{
    public const string MeterName = "QuebecInsider.Metrics";

    private readonly Counter<long> _articleViews;
    private readonly Counter<long> _searches;
    private readonly Counter<long> _eventsProcessed;

    public QuebecInsiderMetrics(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create(MeterName);

        _articleViews = meter.CreateCounter<long>(
            "quebecinsider.articles.views",
            description: "Nombre d'articles consultés");

        _searches = meter.CreateCounter<long>(
            "quebecinsider.searches.count",
            description: "Nombre de recherches effectuées");

        _eventsProcessed = meter.CreateCounter<long>(
            "quebecinsider.events.processed",
            description: "Nombre d'événements asynchrones traités en arrière-plan");
    }

    public void RecordArticleView(string culture, string slug) =>
        _articleViews.Add(1,
            new KeyValuePair<string, object?>("culture", culture),
            new KeyValuePair<string, object?>("slug", slug));

    public void RecordSearch(string culture, string? query = null, int? resultsCount = null) =>
        _searches.Add(1,
            new KeyValuePair<string, object?>("culture", culture),
            new KeyValuePair<string, object?>("has_results", resultsCount > 0));

    public void RecordEventProcessed(string eventType, string culture) =>
        _eventsProcessed.Add(1,
            new KeyValuePair<string, object?>("event_type", eventType),
            new KeyValuePair<string, object?>("culture", culture));
}