namespace RuleExecutor;

using System.Linq.Expressions;
using System.Text.Json;

public class DictionaryExpressionBuilder
{
    public Expression<Func<Dictionary<string, object>, bool>> BuildExpression(FilterCondition filter)
    {
        var parameter = Expression.Parameter(typeof(Dictionary<string, object>), "dict");
        var expression = BuildCondition(parameter, filter);
        return Expression.Lambda<Func<Dictionary<string, object>, bool>>(expression, parameter);
    }

    private Expression BuildCondition(ParameterExpression parameter, FilterCondition condition)
    {
        if (!string.IsNullOrEmpty(condition.Field))
        {
            return BuildFieldExpression(parameter, condition);
        }

        if (condition.Conditions?.Any() == true)
        {
            return BuildLogicalExpression(parameter, condition);
        }

        throw new ArgumentException("Invalid filter condition");
    }

    private Expression BuildFieldExpression(ParameterExpression parameter, FilterCondition condition)
    {
        // Получаем значение из словаря по ключу
        var property = Expression.Property(parameter, "Item", Expression.Constant(condition.Field));
        
        // Для операторов сравнения нужно преобразовать значение
        return condition.Operator?.ToUpper() switch
        {
            "=" => Expression.Equal(GetCast(property, GetClrType(condition.Value)), Expression.Constant(condition.Value)),
            "!=" => Expression.NotEqual(GetCast(property, GetClrType(condition.Value)), Expression.Constant(condition.Value)),
            ">" => BuildComparisonExpression(property, condition, ExpressionType.GreaterThan),
            ">=" => BuildComparisonExpression(property, condition, ExpressionType.GreaterThanOrEqual),
            "<" => BuildComparisonExpression(property, condition, ExpressionType.LessThan),
            "<=" => BuildComparisonExpression(property, condition, ExpressionType.LessThanOrEqual),
            "IN" => BuildInExpression(property, condition.Value),
            "CONTAINS" => BuildContainsExpression(property, condition.Value),
            _ => throw new NotSupportedException($"Operator '{condition.Operator}' is not supported")
        };
    }

    private Expression BuildComparisonExpression(IndexExpression property, FilterCondition condition, ExpressionType expressionType)
    {
        var left = GetCast(property, GetClrType(condition.Value));
        var right = Expression.Constant(condition.Value);
        
        return Expression.MakeBinary(expressionType, left, right);
    }

    private Expression BuildInExpression(IndexExpression property, object? value)
    {
        if (value is not IEnumerable<object>)
            throw new ArgumentException("IN operator requires an array of values");

        var valueList = GetValueArray(value);
        var type = GetClrType(value);
        var containsMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
            .MakeGenericMethod(type);

        return Expression.Call(containsMethod, valueList, GetCast(property, type));
    }

    private Expression BuildContainsExpression(IndexExpression property, object? value)
    {
        var stringValue = value?.ToString() ?? "";
        
        var toStringMethod = typeof(object).GetMethod(nameof(ToString));
        var left = Expression.Call(property, toStringMethod!);
        var right = Expression.Constant(stringValue, typeof(string));
        
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)]);
        return Expression.Call(left, containsMethod!, right);
    }

    private Expression BuildLogicalExpression(ParameterExpression parameter, FilterCondition condition)
    {
        var expressions = condition.Conditions
            .Select(c => BuildCondition(parameter, c))
            .ToList();

        if (!expressions.Any())
            return Expression.Constant(true);

        Expression result = expressions[0];

        for (int i = 1; i < expressions.Count; i++)
        {
            result = condition.LogicalOperator switch
            {
                LogicalOperator.And => Expression.AndAlso(result, expressions[i]),
                LogicalOperator.Or => Expression.OrElse(result, expressions[i]),
                _ => throw new NotSupportedException($"Logical operator '{condition.LogicalOperator}' is not supported")
            };
        }

        return result;
    }
    private Expression GetValueArray(object value)
    {
        if (value is IEnumerable<object> list)
        {
            var type = list.FirstOrDefault()?.GetType() ?? typeof(object);
            
            var expressions = list
                .Select(e => Expression.Constant(e))
                .ToList();

            return Expression.NewArrayInit(type, expressions);
        }
        return Expression.NewArrayInit(typeof(object));
    }

    private Expression GetCast(IndexExpression property, Type type)
    {
        return Expression.Convert(property, type);
    }

    private Type GetClrType(object? property)
    {
        if (property == null)
        {
            return typeof(object);
        }
        if (property is IEnumerable<object> list)
        {
            return list.FirstOrDefault()?.GetType() ?? typeof(object);
        }
        return property.GetType();
    }
}