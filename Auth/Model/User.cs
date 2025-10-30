using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Auth.Model;

public class User
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string Login { get; set; } = string.Empty;
    public byte[] Hash { get; set; }
    public byte[] Salt { get; set; }
}