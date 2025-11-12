using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RuleExecutor;

namespace RuleEditor.Models;

/// <summary>
/// Правило
/// </summary>
public class Rule
{
    /// <summary>
    /// Ид
    /// </summary>
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    [BsonRequired] public required string Name { get; set; }
    /// <summary>
    /// Порядок правила
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Ссылка
    /// </summary>
    public required string Link { get; set; }
    /// <summary>
    /// Условия фильтрации
    /// </summary>
    public required FilterCondition FilterCondition { get; set; }
}