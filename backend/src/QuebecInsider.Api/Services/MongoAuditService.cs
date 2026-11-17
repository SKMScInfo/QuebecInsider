using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace QuebecInsider.Api.Services;

public sealed class AuditLogDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string EventType { get; set; } = string.Empty; // ex: "ArticleCreated", "ArticleViewed", "CacheEvicted"
    public string Actor { get; set; } = string.Empty;     // ex: "admin", "anonymous"
    public string TargetSlug { get; set; } = string.Empty;
    public BsonDocument Metadata { get; set; } = new();
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}

public interface IMongoAuditService
{
    bool IsEnabled { get; }
    Task LogEventAsync(string eventType, string actor, string targetSlug, object metadata, CancellationToken ct = default);
    Task<IReadOnlyList<object>> GetRecentLogsAsync(int limit = 25, CancellationToken ct = default);
}

public sealed class MongoAuditService : IMongoAuditService
{
    private readonly IMongoCollection<AuditLogDocument>? _collection;
    private readonly ILogger<MongoAuditService> _logger;

    public bool IsEnabled => _collection is not null;

    public MongoAuditService(IConfiguration config, ILogger<MongoAuditService> logger)
    {
        _logger = logger;
        var connStr = config.GetConnectionString("MongoDb");
        if (!string.IsNullOrWhiteSpace(connStr))
        {
            var client = new MongoClient(connStr);
            var db = client.GetDatabase("quebecinsider_nosql");
            _collection = db.GetCollection<AuditLogDocument>("audit_logs");
        }
    }

    public async Task LogEventAsync(string eventType, string actor, string targetSlug, object metadata, CancellationToken ct = default)
    {
        if (_collection is null) return;

        try
        {
            var doc = new AuditLogDocument
            {
                EventType = eventType,
                Actor = actor,
                TargetSlug = targetSlug,
                Metadata = metadata.ToBsonDocument(),
                OccurredAtUtc = DateTime.UtcNow
            };
            await _collection.InsertOneAsync(doc, cancellationToken: ct);
            _logger.LogInformation("Document NoSQL inséré dans MongoDB (audit_logs) : {EventType} -> {TargetSlug}", eventType, targetSlug);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MongoDB indisponible lors de l'écriture du log NoSQL (non-bloquant).");
        }
    }

    public async Task<IReadOnlyList<object>> GetRecentLogsAsync(int limit = 25, CancellationToken ct = default)
    {
        if (_collection is null) return [];

        try
        {
            var docs = await _collection
                .Find(Builders<AuditLogDocument>.Filter.Empty)
                .SortByDescending(d => d.OccurredAtUtc)
                .Limit(limit)
                .ToListAsync(ct);

            return docs.Select(d => new
            {
                d.Id,
                d.EventType,
                d.Actor,
                d.TargetSlug,
                d.OccurredAtUtc
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MongoDB indisponible lors de la lecture des logs NoSQL.");
            return [];
        }
    }
}