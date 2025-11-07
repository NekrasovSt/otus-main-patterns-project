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
        cache.Setup(i=>i.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
        object cachedRules = Array.Empty<RuleDto>();
        cache.Setup(i => i.TryGetValue("rules", out cachedRules!)).Returns(true);
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
        cache.Setup(i=>i.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
        object cachedRules =
            new[]
            {
                new RuleDto()
                {
                    Link = "http://another-link.com",
                    Id = "6900b8a5ce7ec3c503c5a3e3",
                    Name = "Rule 1",
                    FilterCondition = new FilterConditionDto()
                    {
                        Field = "field",
                        Operator = "=",
                        Value = "value1"
                    }
                }
            };
        cache.Setup(i => i.TryGetValue("rules", out cachedRules!)).Returns(true);
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
        cache.Setup(i=>i.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
        var cachedRules = new[]
        {
            new RuleDto()
            {
                Link = "http://another-link.com",
                Id = "6900b8a5ce7ec3c503c5a3e3",
                Name = "Rule 1",
                FilterCondition = new FilterConditionDto()
                {
                    Field = "field",
                    Operator = "=",
                    Value = "some value"
                }
            }
        };
        client.Setup(i => i.GetRules(CancellationToken.None)).ReturnsAsync(cachedRules);
        var service = new LinkRedirector(client.Object, cache.Object);

        var dict = new Dictionary<string, object>()
        {
            { "field", "some value" },
        };
        var link = await service.RedirectAsync(dict, CancellationToken.None);
        Assert.Equal("http://another-link.com", link);
    }
}