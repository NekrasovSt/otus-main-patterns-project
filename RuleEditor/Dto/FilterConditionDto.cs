using System.Text.Json.Serialization;
using RuleExecutor;
using ServiceUtils.Serialization;

namespace RuleEditor.Dto;

/// <summary>
/// Фильтр
/// </summary>
public class FilterConditionDto
{
    /// <summary>
    /// Логическая операция
    /// </summary>
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;

    /// <summary>
    /// Коллекция вложенных условий
    /// </summary>
    public List<FilterConditionDto> Conditions { get; set; } = new();

    /// <summary>
    /// Параметр для фильтра, работает толь если нет Conditions
    /// </summary>
    public string? Field { get; set; }

    /// <summary>
    /// Оператор сравнения
    /// </summary>
    public string? Operator { get; set; }

    /// <summary>
    /// Значение для сравнения
    /// </summary>
    [JsonConverter(typeof(FlexibleObjectConverter))]
    public object? Value { get; set; }
}