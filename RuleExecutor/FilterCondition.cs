namespace RuleExecutor;

public class FilterCondition
{
    public LogicalOperator LogicalOperator { get; set; } = LogicalOperator.And;
    public List<FilterCondition> Conditions { get; set; } = new();
    public string? Field { get; set; }
    public string? Operator { get; set; }
    public object? Value { get; set; }
}