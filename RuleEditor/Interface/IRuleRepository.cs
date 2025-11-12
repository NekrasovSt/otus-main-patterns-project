using RuleEditor.Models;

namespace RuleEditor.Interface;

/// <summary>
/// Доступ к правилам
/// </summary>
public interface IRuleRepository
{
    /// <summary>
    /// Все правила
    /// </summary>
    Task<IEnumerable<Rule>> GetAllAsync(CancellationToken token);

    /// <summary>
    /// Правило по ИД
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
    /// Инициализация
    /// </summary>
    Task InitDBAsync();

    /// <summary>
    /// Обновить
    /// </summary>
    Task<Rule> UpdateAsync(Rule rule, CancellationToken token);
}