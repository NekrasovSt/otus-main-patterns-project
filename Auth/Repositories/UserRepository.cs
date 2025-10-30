using System.Data;
using Auth.Helpers;
using Auth.Interfaces;
using Auth.Model;
using MongoDB.Driver;
using ServiceUtils.Exceptions;

namespace Auth.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoClient _client;

    public UserRepository(IMongoClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task InitDBAsync()
    {
        var database = _client.GetDatabase("users");
        var list = (await database.ListCollectionNamesAsync()).ToList();
        const string indexName = "user_Login_idx";
        if (!list.Contains(nameof(User)))
        {
            PasswordHasher.CreatePasswordHash("admin", out var passwordHash, out var passwordSalt);
            var user = new User()
            {
                Login = "admin",
                Hash = passwordHash,
                Salt = passwordSalt,
            };
            await database.GetCollection<User>(nameof(User)).InsertOneAsync(user);
        }

        var collection = database.GetCollection<User>(nameof(User));
        var indexes = await collection.Indexes.ListAsync();
        var indexList = await indexes.ToListAsync();
        if (!indexList.Any(index => index["name"].AsString.Equals(indexName, StringComparison.OrdinalIgnoreCase)))
        {
            var uniqueIndex = new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(i => i.Login),
                new CreateIndexOptions
                {
                    Unique = true, Name = indexName,
                    Collation = new Collation("en", strength: CollationStrength.Primary)
                });

            await collection.Indexes.CreateOneAsync(uniqueIndex);
        }
    }

    public Task<User?> GetUserAsync(string login, CancellationToken token)
    {
        var database = _client.GetDatabase("users");
        return database.GetCollection<User?>(nameof(User)).Find(p => p.Login == login)
            .FirstOrDefaultAsync(token);
    }

    public async Task<User?> UpdateUserAsync(User user, CancellationToken token)
    {
        var database = _client.GetDatabase("users");
        return await database.GetCollection<User>(nameof(User))
            .FindOneAndReplaceAsync(p => p.Id == user.Id, user, new() { ReturnDocument = ReturnDocument.After },
                cancellationToken: token);
    }

    public async Task<User> AddUserAsync(User user, CancellationToken token)
    {
        var database = _client.GetDatabase("users");
        try
        {
            await database.GetCollection<User>(nameof(User)).InsertOneAsync(user, cancellationToken: token);
        }
        catch (MongoWriteException e) when (e.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new EntityAlreadyExistException($"Пользователь с именем {user.Login} уже существует")
            {
                PropertyName = nameof(User.Login)
            };
        }

        return user;
    }
}