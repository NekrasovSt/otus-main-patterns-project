using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.Model;

/// <summary>
/// Пользователь
/// </summary>
public class User
{
    /// <summary>
    /// Ид
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Логин
    /// </summary>
    public required string Login { get; set; } = string.Empty;

    /// <summary>
    /// Хэш
    /// </summary>
    public required byte[] Hash { get; set; }

    /// <summary>
    /// Соль
    /// </summary>
    public required byte[] Salt { get; set; }
}