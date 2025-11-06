using System.Text.Json.Serialization;
using ServiceUtils.Serialization;

namespace LinkServer.Dto;

public class FilterConditionDto
{
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;
    public List<FilterConditionDto> Conditions { get; set; } = new();
    public string? Field { get; set; }
    public string? Operator { get; set; }
    [JsonConverter(typeof(FlexibleObjectConverter))]
    public object? Value { get; set; }
}