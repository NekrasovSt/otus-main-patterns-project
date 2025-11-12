using HistoryService.Interfaces;
using HistoryService.Models;
using Mapster;
using ServiceUtils.Broker;
using MassTransit;

namespace HistoryService.Service;

/// <inheritdoc />
public class HistoryConsumer(IHistoryRepository historyRepository) : IConsumer<RuleExecutedDto>
{
    private readonly IHistoryRepository _historyRepository = historyRepository ?? throw new ArgumentNullException(nameof(historyRepository));

    /// <inheritdoc />
    public Task Consume(ConsumeContext<RuleExecutedDto> context)
    {
        return _historyRepository.AddHistoryAsync(context.Message.Adapt<RuleExecuted>(), context.CancellationToken);
    }
}