using System.Validation;
using RuleExecutor;

namespace RuleEditor.Services;

public class FilterConditionValidator : FlatValidator<FilterCondition>
{
    private string[] Operators = ["=", "!=", "IN", "CONTAINS", ">", "<", "<=", ">="];

    public FilterConditionValidator()
    {
        ErrorIf(i => i.Operator.IsEmpty(), $"Поле {nameof(FilterCondition.Operator)} не должно быть пустым",
            i => i.Operator);
        ValidIf(i => Operators.Contains(i.Operator), "Недопустимый оператор", i => i.Operator);
        When(i => i.Conditions.Count == 0,
            then =>
            {
                ErrorIf(i => i.Field.IsEmpty(),
                    $"Поле {nameof(FilterCondition.Field)} не должно быть пустым если нет {nameof(FilterCondition.Conditions)}",
                    i => i.Field);
            },
            @else =>
            {
                ValidIf(i => i.Field.IsEmpty(),
                    $"Поле {nameof(FilterCondition.Field)} должно быть пустым если есть {nameof(FilterCondition.Conditions)}",
                    i => i.Field);
            });
    }
}