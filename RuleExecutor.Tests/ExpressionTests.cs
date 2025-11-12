using System.Text.Json;
using System.Text.Json.Serialization;

namespace RuleExecutor.Tests;

public class ExpressionTests
{
    private Func<Dictionary<string, object>, bool> GetExpression(FilterCondition filterData)
    {
        var builder = new DictionaryExpressionBuilder();
        var expression = builder.BuildExpression(filterData);
        return expression.Compile();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void SimpleBool(bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "isMobile",
            Operator = "=",
            Value = true
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "isMobile", resultValue }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(10, false)]
    public void SimpleIntEq(int value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "version",
            Operator = "=",
            Value = 3
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "version", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData(3, false)]
    [InlineData(10, true)]
    [InlineData(99, true)]
    public void SimpleIntNotEq(int value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "version",
            Operator = "!=",
            Value = 3
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "version", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData(3, false)]
    [InlineData(10, true)]
    [InlineData(99, true)]
    public void SimpleIntGt(int value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "version",
            Operator = ">",
            Value = 5
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "version", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData(3, false)]
    [InlineData(5, true)]
    [InlineData(10, true)]
    [InlineData(99, true)]
    public void SimpleIntGtEq(int value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "version",
            Operator = ">=",
            Value = 5
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "version", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(10, false)]
    [InlineData(99, false)]
    public void SimpleIntLt(int value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "version",
            Operator = "<",
            Value = 5
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "version", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData(3, true)]
    [InlineData(5, true)]
    [InlineData(10, false)]
    [InlineData(99, false)]
    public void SimpleIntLtEq(int value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "version",
            Operator = "<=",
            Value = 5
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "version", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData("safari", true)]
    [InlineData("edge", false)]
    public void SimpleString(string value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "browser",
            Operator = "=",
            Value = "safari"
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "browser", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData("safari", true)]
    [InlineData("mobile-safari", true)]
    [InlineData("edge", false)]
    public void ContainsString(string value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "browser",
            Operator = "CONTAINS",
            Value = "ri"
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "browser", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData("safari", false)]
    [InlineData("ie", false)]
    [InlineData("edge", true)]
    public void InString(string value, bool resultValue)
    {
        var json = new FilterCondition()
        {
            Field = "browser",
            Operator = "IN",
            Value = new[] { "edge", "firefox", "opera" }
        };
        var func = GetExpression(json);

        var dict = new Dictionary<string, object>()
        {
            { "browser", value }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData("safari", 10, false)]
    [InlineData("edge", 4, false)]
    [InlineData("edge", 6, true)]
    public void ComplexTwoConditions(string browser, int version, bool resultValue)
    {
        var json = new FilterCondition()
        {
            LogicalOperator = LogicalOperator.And,
            Conditions =
            [
                new()
                {
                    Field = "browser",
                    Operator = "=",
                    Value = "edge"
                },
                new()
                {
                    Field = "version",
                    Operator = ">",
                    Value = 5
                }
            ]
        };
        var func = GetExpression(json);
        var dict = new Dictionary<string, object>()
        {
            { "browser", browser },
            { "version", version }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData("safari", 10, true)]
    [InlineData("edge", 4, true)]
    [InlineData("edge", 6, true)]
    public void ComplexTwoOrConditions(string browser, int version, bool resultValue)
    {
        var json = new FilterCondition()
        {
            LogicalOperator = LogicalOperator.Or,
            Conditions =
            [
                new()
                {
                    Field = "browser",
                    Operator = "=",
                    Value = "edge"
                },
                new()
                {
                    Field = "version",
                    Operator = ">",
                    Value = 5
                }
            ]
        };
        var func = GetExpression(json);
        var dict = new Dictionary<string, object>()
        {
            { "browser", browser },
            { "version", version }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }

    [Theory]
    [InlineData(false, "ios", false)]
    [InlineData(true, "ios", false)]
    [InlineData(true, "Android", true)]
    [InlineData(false, "Windows", true)]
    public void ComplexFourOrConditions(bool isMobile, string os, bool resultValue)
    {
        var json = new FilterCondition()
        {
            LogicalOperator = LogicalOperator.Or,
            Conditions =
            [
                new()
                {
                    LogicalOperator = LogicalOperator.And,
                    Conditions =
                    [
                        new()
                        {
                            Field = "isMobile",
                            Operator = "=",
                            Value = true
                        },
                        new()
                        {
                            Field = "os",
                            Operator = "=",
                            Value = "Android"
                        }
                    ]
                },
                new()
                {
                    LogicalOperator = LogicalOperator.And,
                    Conditions =
                    [
                        new()
                        {
                            Field = "isMobile",
                            Operator = "=",
                            Value = false
                        },
                        new()
                        {
                            Field = "os",
                            Operator = "=",
                            Value = "Windows"
                        }
                    ]
                }
            ]
        };
        var func = GetExpression(json);
        var dict = new Dictionary<string, object>()
        {
            { "isMobile", isMobile },
            { "os", os }
        };
        var result = func(dict);
        Assert.Equal(resultValue, result);
    }
}