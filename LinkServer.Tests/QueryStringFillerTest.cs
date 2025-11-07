using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;

namespace LinkServer.Tests;

public class QueryStringFillerTest
{
    [Fact]
    public void EmptyQuery()
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(i => i.HttpContext!.Request.Query).Returns(new TestQueryCollection(new Dictionary<string, StringValues>()));
        var service = new QueryStringFiller(accessor.Object);
        var dictionary = service.Fill();
        Assert.Empty(dictionary);
    }
    
    [Fact]
    public void FieldsExist()
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(i => i.HttpContext!.Request.Query).Returns(new TestQueryCollection(new Dictionary<string, StringValues>()
        {
            {"param1", new StringValues("value")},
            {"param2", new StringValues(["value1", "value2"])}
        }));
        var service = new QueryStringFiller(accessor.Object);
        var dictionary = service.Fill();
        Assert.Equal("value", dictionary["$q:param1"]);
        Assert.Equal("value1,value2", dictionary["$q:param2"]);
        Assert.Equal(2, dictionary.Count);
    }
}