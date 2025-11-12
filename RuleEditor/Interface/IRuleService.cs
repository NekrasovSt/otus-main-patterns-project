using RuleEditor.Models;

namespace RuleEditor.Interface;

/// <summary>
/// Сервис управления правилами
/// </summary>
public interface IRuleService
{
    /// <summary>
    /// Получить все правила
    /// </summary>
    Task<IEnumerable<Rule>> GetAllAsync(CancellationToken token);

    /// <summary>
    /// Получить правило
    /// </summary>
    Task<Rule> GetAsync(string id, CancellationToken token);

    /// <summary>
    /// Удалить правило
    /// </summary>
    Task DeleteAsync(string id, CancellationToken token);

    /// <summary>
    /// Добавить правило
    /// </summary>
    Task<Rule> AddAsync(Rule newRule, CancellationToken token);

    /// <summary>
    /// Обновить правило
    /// </summary>
    Task<Rule> UpdateAsync(Rule rule, CancellationToken token);
}