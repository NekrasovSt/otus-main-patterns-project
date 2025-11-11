using System.Text.Json;
using ServiceUtils.Tests.Dto;

namespace ServiceUtils.Tests;

public class ConvertorTests
{
    [Theory]
    [InlineData(typeof(int), "{\"Value\": 100 }")]
    [InlineData(typeof(long), "{\"Value\": 9223372036854775807  }")]
    [InlineData(typeof(string), "{\"Value\": \"Hello\" }")]
    [InlineData(typeof(bool), "{\"Value\": true }")]
    [InlineData(typeof(bool), "{\"Value\": false }")]
    [InlineData(typeof(double), "{\"Value\": 10.5 }")]
    [InlineData(typeof(List<object>), "{\"Value\": [10.5] }")]
    [InlineData(typeof(List<object>), "{\"Value\": [10] }")]
    [InlineData(typeof(JsonElement), "{\"Value\": {} }")]
    public void CheckTypes(Type type, string json)
    {
        var restoredPerson = JsonSerializer.Deserialize<NewObjDto>(json);
        Assert.NotNull(restoredPerson);
        Assert.NotNull(restoredPerson.Value);
        Assert.Equal(type, restoredPerson.Value.GetType());
    }

    [Fact]
    public void CheckNull()
    {
        var restoredPerson = JsonSerializer.Deserialize<NewObjDto>("{\"Value\": null }");
        Assert.NotNull(restoredPerson);
        Assert.Null(restoredPerson.Value); 
    }

    [Fact]
    public void Serialize()
    {
        var restoredPerson = JsonSerializer.Serialize(new NewObjDto { Value = 123 });
        Assert.Equal("{\"Value\":123}", restoredPerson);
    }
}