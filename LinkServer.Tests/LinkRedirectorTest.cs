using LinkServer.Dto;
using LinkServer.Middleware;
using LinkServer.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace LinkServer.Tests;

public class LinkRedirectorTest
{
    [Fact]
    public async Task EmptyRules()
    {
        var cache = new Mock<IMemoryCache>();
        object cachedRules = Array.Empty<RuleDto>();
        cache.Setup(i => i.TryGetValue("rules", out cachedRules)).Returns(true);
        var service = new LinkRedirector(Mock.Of<IRuleEditorClient>(), cache.Object);

        var dict = new Dictionary<string, object>()
        {
            { "key1", "value1" },
        };
        var link = await service.RedirectAsync(dict, CancellationToken.None);
        Assert.Equal(LinkRedirector.DefaultLink, link);
    }

    [Fact]
    public async Task RuleDoesNotWork()
    {
        var client = new Mock<IRuleEditorClient>();
        var cache = new Mock<IMemoryCache>();
        object cachedRules =
            new[]
            {
                new RuleDto()
                {
                    Link = "http://another-link.com",
                    FilterCondition = new FilterConditionDto()
                    {
                        Field = "field",
                        Operator = "=",
                        Value = "value1"
                    }
                }
            };
        cache.Setup(i => i.TryGetValue("rules", out cachedRules)).Returns(true);
        var service = new LinkRedirector(client.Object, cache.Object);

        var dict = new Dictionary<string, object>()
        {
            { "field", "some value" },
        };
        var link = await service.RedirectAsync(dict, CancellationToken.None);
        Assert.Equal(LinkRedirector.DefaultLink, link);
    }

    [Fact]
    public async Task RuleWorks()
    {
        var client = new Mock<IRuleEditorClient>();
        var cache = new Mock<IMemoryCache>();
        object cachedRules = new[]
        {
            new RuleDto()
            {
                Link = "http://another-link.com",
                FilterCondition = new FilterConditionDto()
                {
                    Field = "field",
                    Operator = "=",
                    Value = "some value"
                }
            }
        };
        cache.Setup(i => i.TryGetValue("rules", out cachedRules)).Returns(true);
        var service = new LinkRedirector(client.Object, cache.Object);

        var dict = new Dictionary<string, object>()
        {
            { "field", "some value" },
        };
        var link = await service.RedirectAsync(dict, CancellationToken.None);
        Assert.Equal("http://another-link.com", link);
    }
}