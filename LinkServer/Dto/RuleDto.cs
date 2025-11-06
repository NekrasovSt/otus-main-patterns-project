using System.ComponentModel.DataAnnotations;

namespace LinkServer.Dto;

/// <summary>
/// Правило
/// </summary>
public class RuleDto
{
    /// <summary>
    /// Ид
    /// </summary>
    [Length(24, 24)]
    [RegularExpression("[0123456789abcdefABCDEF]{24}")]
    public string? Id { get; set; }

    /// <summary>
    /// Название
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Порядок проверки правил
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Ссылка для перехода
    /// </summary>
    public string Link { get; set; }

    /// <summary>
    /// Фильтры
    /// </summary>
    public FilterConditionDto FilterCondition { get; set; }
}