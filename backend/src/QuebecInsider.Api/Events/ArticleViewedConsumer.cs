using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuebecInsider.Api.Data;
using QuebecInsider.Api.Hubs;
using QuebecInsider.Api.Services;
using Microsoft.AspNetCore.OutputCaching;
namespace QuebecInsider.Api.Events;

public sealed class ArticleViewedConsumer(
    QuebecInsiderDbContext db,
    IArticleService articleService,
    IHubContext<TrendingHub> hubContext,
    QuebecInsiderMetrics metrics,
    IOutputCacheStore cacheStore,
    ILogger<ArticleViewedConsumer> logger,
    ISnowflakeAnalyticsService snowflake) : IConsumer<ArticleViewedEvent>
{
    public async Task Consume(ConsumeContext<ArticleViewedEvent> context)
    {
        var msg = context.Message;

        var article = await db.Articles
            .FirstOrDefaultAsync(a => a.Id == msg.ArticleId, context.CancellationToken);

        if (article is null)
        {
            return;
        }

        article.PopularityScore += 1;
        await db.SaveChangesAsync(context.CancellationToken);
        await cacheStore.EvictByTagAsync("trending", context.CancellationToken);

        metrics.RecordEventProcessed("ArticleViewed", msg.Culture);

        logger.LogInformation(
            "Événement asynchrone traité : Article #{ArticleId} ({Slug}) -> Nouveau PopularityScore = {Score}",
            article.Id, msg.Slug, article.PopularityScore);

        var trending = await articleService.GetTrendingAsync(msg.Culture, context.CancellationToken);
        await hubContext.Clients.All.SendAsync("TrendingUpdated", trending, context.CancellationToken);
        await snowflake.TrackArticleViewAsync(msg, context.CancellationToken);
    }
}