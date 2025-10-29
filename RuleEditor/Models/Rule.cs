using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RuleExecutor;

namespace RuleEditor.Models;

public class Rule
{
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonRequired] public string Name { get; set; }
    public int Order { get; set; }

    public string Link { get; set; }
    public FilterCondition? FilterCondition { get; set; }
}