using MongoDB.Bson;

namespace HistoryService.Models;

/// <summary>
/// Запись о выполненом правиле
/// </summary>
public class RuleExecutedOrm
{
    /// <summary>
    /// Ид
    /// </summary>
    public ObjectId Id { get; set; }

    /// <summary>
    /// Ид правила
    /// </summary>
    public required string RuleId { get; set; }

    /// <summary>
    /// Название правила
    /// </summary>
    public required string RuleName { get; set; }

    /// <summary>
    /// Время выполнения правила UTC
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    /// Url редиректа
    /// </summary>
    public required string Url { get; set; }
}