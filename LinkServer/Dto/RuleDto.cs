namespace LinkServer.Dto;

public class RuleDto
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    
    public string Link { get; set; }
    public FilterConditionDto  FilterCondition { get; set; }
}