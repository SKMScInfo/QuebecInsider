using System.Data;
using QuebecInsider.Api.Events;
using Snowflake.Data.Client;

namespace QuebecInsider.Api.Services;

public interface ISnowflakeAnalyticsService
{
    Task TrackArticleViewAsync(ArticleViewedEvent evt, CancellationToken cancellationToken);
}

public sealed class SnowflakeAnalyticsService(
    IConfiguration configuration,
    ILogger<SnowflakeAnalyticsService> logger) : ISnowflakeAnalyticsService
{
    public async Task TrackArticleViewAsync(ArticleViewedEvent evt, CancellationToken cancellationToken)
    {
        var connectionString = configuration.GetConnectionString("Snowflake");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return;
        }

        try
        {
            await using var conn = new SnowflakeDbConnection { ConnectionString = connectionString };
            await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                INSERT INTO QUEBECINSIDER_DW.PUBLIC.FACT_ARTICLE_VIEWS (ARTICLE_ID, CULTURE, SLUG, VIEWED_AT_UTC)
                VALUES (?, ?, ?, ?);
                """;

            AddParam(cmd, "1", DbType.Int32, evt.ArticleId);
            AddParam(cmd, "2", DbType.String, evt.Culture);
            AddParam(cmd, "3", DbType.String, evt.Slug);
            AddParam(cmd, "4", DbType.String, evt.ViewedAtUtc.ToString("O"));

            await cmd.ExecuteNonQueryAsync(cancellationToken);
            logger.LogInformation("Événement analytique envoyé dans Snowflake Data Warehouse : {Slug}", evt.Slug);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Impossible d'envoyer l'événement vers Snowflake (non-bloquant).");
        }
    }

    private static void AddParam(IDbCommand cmd, string name, DbType type, object value)
    {
        var param = cmd.CreateParameter();
        param.ParameterName = name;
        param.DbType = type;
        param.Value = value;
        cmd.Parameters.Add(param);
    }
}