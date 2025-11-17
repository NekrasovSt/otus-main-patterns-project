using MongoDB.Driver;
using RuleEditor.Interface;
using RuleEditor.Models;
using RuleExecutor;
using ServiceUtils.Exceptions;

namespace RuleEditor.Repositories;

/// <inheritdoc />
public class RuleRepository : IRuleRepository
{
    private readonly IMongoClient _client;

    /// <summary>
    /// Конструктор
    /// </summary>
    public RuleRepository(IMongoClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Rule>> GetAllAsync(CancellationToken token)
    {
        var database = _client.GetDatabase("rules");
        var result = await database.GetCollection<Rule>(nameof(Rule)).Find("{}")
            .SortBy(i => i.Order).ToListAsync(token);
        return result;
    }

    /// <inheritdoc />
    public async Task<Rule> GetAsync(string id, CancellationToken token)
    {
        var database = _client.GetDatabase("rules");
        var rule = await database.GetCollection<Rule>(nameof(Rule))
            .Find(p => p.Id == id)
            .FirstOrDefaultAsync(token);
        if (rule == null)
        {
            throw new EntityNotFoundException(id);
        }

        return rule;
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id, CancellationToken token)
    {
        var database = _client.GetDatabase("rules");
        var rule = await database.GetCollection<Rule>(nameof(Rule))
            .FindOneAndDeleteAsync(p => p.Id == id, cancellationToken: token);
        if (rule == null)
        {
            throw new EntityNotFoundException(id);
        }
    }

    /// <inheritdoc />
    public async Task<Rule> AddAsync(Rule newRule, CancellationToken token)
    {
        newRule.Id = null;
        var database = _client.GetDatabase("rules");
        if (newRule.Order == 0)
        {
            var list = await database.GetCollection<Rule>(nameof(Rule)).Aggregate().SortByDescending((a) => a.Order)
                .Limit(1)
                .FirstAsync(token);
            newRule.Order = list?.Order ?? 0;
        }

        try
        {
            await database.GetCollection<Rule>(nameof(Rule)).InsertOneAsync(newRule, cancellationToken: token);
        }
        catch (MongoWriteException e) when (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new EntityAlreadyExistException($"Правило с именем {newRule.Name} уже существует")
            {
                PropertyName = nameof(Rule.Name)
            };
        }

        return newRule;
    }

    /// <inheritdoc />
    public async Task InitDBAsync()
    {
        var database = _client.GetDatabase("rules");
        var list = (await database.ListCollectionNamesAsync()).ToList();
        const string indexName = "rule_Name_idx";
        if (!list.Contains(nameof(Rule)))
        {
            await database.CreateCollectionAsync(nameof(Rule));

            var newRule = new Rule
            {
                Order = 0,
                Name = "Demo rule 1",
                FilterCondition = new FilterCondition()
                {
                    Operator = "IN",
                    Field = "preferredLanguage",
                    Value = new[] { "fr" },
                },
                Link = "https://www.fr.com",
            };
            await database.GetCollection<Rule>(nameof(Rule)).InsertOneAsync(newRule);
            newRule = new Rule
            {
                Order = 1,
                Name = "Демо правило 2",
                FilterCondition = new FilterCondition()
                {
                    Operator = "!=",
                    Field = "browser",
                    Value = "Chrome",
                },
                Link = "http://www.yandex.ru",
            };
            await database.GetCollection<Rule>(nameof(Rule)).InsertOneAsync(newRule);
            newRule = new Rule
            {
                Order = 2,
                Name = "Демо правило 3",
                Link = "http://www.google.ru",
                FilterCondition = new FilterCondition()
                {
                    Operator = "=",
                    Field = "os",
                    Value = "Win 64",
                }
            };
            await database.GetCollection<Rule>(nameof(Rule)).InsertOneAsync(newRule);
        }

        var collection = database.GetCollection<Rule>(nameof(Rule));
        var indexes = await collection.Indexes.ListAsync();
        var indexList = await indexes.ToListAsync();
        if (!indexList.Any(index => index["name"].AsString.Equals(indexName, StringComparison.OrdinalIgnoreCase)))
        {
            var uniqueIndex = new CreateIndexModel<Rule>(Builders<Rule>.IndexKeys.Ascending(i => i.Name),
                new CreateIndexOptions { Unique = true, Name = indexName, Collation = new Collation("ru", strength: CollationStrength.Primary)});

            await collection.Indexes.CreateOneAsync(uniqueIndex);
        }
    }

    /// <inheritdoc />
    public async Task<Rule> UpdateAsync(Rule rule, CancellationToken token)
    {
        if (rule.Id == null)
        {
            throw new NullReferenceException(nameof(rule.Id));
        }
        var database = _client.GetDatabase("rules");
        var updatedRule = await database.GetCollection<Rule>(nameof(Rule))
            .FindOneAndReplaceAsync(p => p.Id == rule.Id, rule, new() { ReturnDocument = ReturnDocument.After }, cancellationToken: token);
        if (updatedRule == null)
        {
            throw new EntityNotFoundException(rule.Id);
        }
        return updatedRule;
    }
}