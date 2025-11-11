using Mongo2Go;
using MongoDB.Driver;
using RuleEditor.Models;
using RuleEditor.Repositories;
using RuleExecutor;
using ServiceUtils.Exceptions;

namespace RuleEditor.Tests;

public class RuleRepositoryTests
{
    [Fact]
    public async Task Init()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

        var database = client.GetDatabase("rules");
        var result = await database.GetCollection<Rule>(nameof(Rule)).Find("{}").ToListAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAll()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

        var allAsync = await repository.GetAllAsync(CancellationToken.None);
        Assert.Equal(3, allAsync.Count());
    }

    [Fact]
    public async Task GetNotFound()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

        await Assert.ThrowsAsync<EntityNotException>(() =>
            repository.GetAsync("6900b8a5ce7ec3c503c5a3e3", CancellationToken.None));
    }

    [Fact]
    public async Task GetOk()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();
        var database = client.GetDatabase("rules");
        var result = await database
            .GetCollection<Rule>(nameof(Rule)).Find("{}")
            .FirstOrDefaultAsync();

        Assert.NotNull(result);
        var rule = await repository.GetAsync(result.Id!, CancellationToken.None);

        Assert.NotNull(rule);
    }

    [Fact]
    public async Task DeleteNotFound()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

        await Assert.ThrowsAsync<EntityNotException>(() =>
            repository.DeleteAsync("6900b8a5ce7ec3c503c5a3e3", CancellationToken.None));
    }

    [Fact]
    public async Task DeleteOk()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

        var database = client.GetDatabase("rules");
        var result = await database
            .GetCollection<Rule>(nameof(Rule)).Find("{}")
            .FirstOrDefaultAsync();

        Assert.NotNull(result);
        await repository.DeleteAsync(result.Id!, CancellationToken.None);

        var all = await database
            .GetCollection<Rule>(nameof(Rule)).Find("{}")
            .ToListAsync();
        Assert.Equal(2, all.Count());
    }

    [Fact]
    public async Task AddNotUnique()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

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
        await Assert.ThrowsAsync<EntityAlreadyExistException>(() =>
            repository.AddAsync(newRule, CancellationToken.None));
    }
    
    [Fact]
    public async Task AddOk()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

        var newRule = new Rule
        {
            Order = 0,
            Name = "Demo rule 9",
            FilterCondition = new FilterCondition()
            {
                Operator = "IN",
                Field = "preferredLanguage",
                Value = new[] { "fr" },
            },
            Link = "https://www.fr.com",
        };
        await repository.AddAsync(newRule, CancellationToken.None);
        
        var database = client.GetDatabase("rules");
        var result = await database.GetCollection<Rule>(nameof(Rule)).Find("{}").ToListAsync();

        Assert.Equal(4, result.Count);
    }
    
    [Fact]
    public async Task UpdateNotFound()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

        var newRule = new Rule
        {
            Id = "6900b8a5ce7ec3c503c5a3e3",
            Order = 0,
            Name = "Demo rule 9",
            FilterCondition = new FilterCondition()
            {
                Operator = "IN",
                Field = "preferredLanguage",
                Value = new[] { "fr" },
            },
            Link = "https://www.fr.com",
        };
        
        await Assert.ThrowsAsync<EntityNotException>(() =>
            repository.UpdateAsync(newRule, CancellationToken.None));
    }
    
    [Fact]
    public async Task UpdateOk()
    {
        using MongoDbRunner runner = MongoDbRunner.Start();
        var client = new MongoClient(runner.ConnectionString);
        var repository = new RuleRepository(client);

        await repository.InitDBAsync();

        var newRule = new Rule
        {
            Id = "6900b8a5ce7ec3c503c5a3e3",
            Order = 0,
            Name = "Demo rule 9",
            FilterCondition = new FilterCondition()
            {
                Operator = "IN",
                Field = "preferredLanguage",
                Value = new[] { "fr" },
            },
            Link = "https://www.fr.com",
        };
        
        var database = client.GetDatabase("rules");
        var result = await database
            .GetCollection<Rule>(nameof(Rule)).Find("{}")
            .FirstOrDefaultAsync();

        newRule.Id = result.Id;
        
        await repository.UpdateAsync(newRule, CancellationToken.None);
    }
}