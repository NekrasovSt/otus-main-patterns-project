using System.Validation;
using RuleEditor.Models;

namespace RuleEditor.Services;

/// <summary>
/// Валидация правила
/// </summary>
public class RuleValidator : FlatValidator<Rule>
{
    /// <summary>
    /// Конструктор
    /// </summary>
    public RuleValidator()
    {
        ValidIf(m => m.Link.IsAbsoluteUri(), $"Поле {nameof(Rule.Link)} должно быть валидным url", m => m.Link);
        ErrorIf(m => m.Link.IsEmpty(), $"Поле {nameof(Rule.Link)} не может быть пустым", m => m.Link);
        ErrorIf(m => m.Name.IsEmpty(), $"Поле {nameof(Rule.Name)} не может быть пустым", m => m.Name);
        ErrorIf(m => m.FilterCondition == null, $"Поле {nameof(Rule.FilterCondition)} не может быть пустым", m => m.FilterCondition);
    }
}