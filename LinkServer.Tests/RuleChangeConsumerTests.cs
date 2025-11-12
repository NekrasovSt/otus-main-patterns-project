using LinkServer.Services;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using ServiceUtils.Broker;

namespace LinkServer.Tests;

public class RuleChangeConsumerTests
{
    [Fact]
    public async Task RemoveCache()
    {
        var cache = new Mock<IMemoryCache>();
        var consumer = new RuleChangeConsumer(cache.Object);

        await consumer.Consume(Mock.Of<ConsumeContext<RuleChangedDto>>());
        
        cache.Verify(x => x.Remove("rules"), Times.Once);
    }

    [Fact]
    public void NullArgument()
    {
        Assert.Throws<ArgumentNullException>(() => new RuleChangeConsumer((IMemoryCache)null!));
    }
}