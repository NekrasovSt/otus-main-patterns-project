using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using ServiceUtils.Broker;

namespace LinkServer.Services;

public class RuleChangeConsumer: IConsumer<RuleChangedDto>
{
    private readonly IMemoryCache _memoryCache;

    public RuleChangeConsumer(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public Task Consume(ConsumeContext<RuleChangedDto> context)
    {
        _memoryCache.Remove("rules");
        return Task.CompletedTask;
    }
}