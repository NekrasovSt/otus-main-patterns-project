using HistoryService.Interfaces;
using HistoryService.Models;
using MongoDB.Driver;

namespace HistoryService.Repositories;

/// <inheritdoc />
public class HistoryRepository(IMongoClient client) : IHistoryRepository
{
    private readonly IMongoClient _client = client ?? throw new ArgumentNullException(nameof(client));

    /// <inheritdoc />
    public Task AddHistoryAsync(RuleExecuted ruleExecuted, CancellationToken token)
    {
        ruleExecuted.Id = null;
        var database = _client.GetDatabase("history");
        return database.GetCollection<RuleExecuted>(nameof(RuleExecuted))
            .InsertOneAsync(ruleExecuted, cancellationToken: token);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RuleExecuted>> GetHistoryAsync(CancellationToken token)
    {
        var database = _client.GetDatabase("history");
        var result = await database.GetCollection<RuleExecuted>(nameof(RuleExecuted)).Find("{}")
            .ToListAsync(token);
        return result;
    }
}