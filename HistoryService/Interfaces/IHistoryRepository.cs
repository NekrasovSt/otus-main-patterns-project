using HistoryService.Models;

namespace HistoryService.Interfaces;

/// <summary>
/// Доступ к истории
/// </summary>
public interface IHistoryRepository
{
    /// <summary>
    /// Добавить запись
    /// </summary>
    Task AddHistoryAsync(RuleExecuted ruleExecuted, CancellationToken token);
    
    /// <summary>
    /// Получить историю
    /// </summary>
    Task<IEnumerable<RuleExecuted>> GetHistoryAsync(CancellationToken token);
}