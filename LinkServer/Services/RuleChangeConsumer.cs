using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using ServiceUtils.Broker;

namespace LinkServer.Services;

/// <inheritdoc />
public class RuleChangeConsumer: IConsumer<RuleChangedDto>
{
    private readonly IMemoryCache _memoryCache;

    /// <summary>
    /// Конструктор
    /// </summary>
    public RuleChangeConsumer(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    /// <inheritdoc />
    public Task Consume(ConsumeContext<RuleChangedDto> context)
    {
        _memoryCache.Remove("rules");
        return Task.CompletedTask;
    }
}