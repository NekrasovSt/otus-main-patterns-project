using HistoryService.Context;
using HistoryService.Models;
using HotChocolate.Authorization;

namespace HistoryService.Queries;

/// <summary>
/// Запрос истории
/// </summary>
public class HistoryQuery
{
    /// <summary>
    /// История выполнения правил
    /// </summary>
    [UsePaging]
    [UseProjection] 
    [UseFiltering] 
    [UseSorting]
    [Authorize(Policy = "Default")]
    public IQueryable<RuleExecutedOrm> Executions([Service] ApplicationContext context) => context.Executions;
}