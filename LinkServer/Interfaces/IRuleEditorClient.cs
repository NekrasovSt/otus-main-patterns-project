using LinkServer.Dto;

namespace LinkServer.Services;

/// <summary>
/// Клиент для получения правил
/// </summary>
public interface IRuleEditorClient
{
    /// <summary>
    /// Все правила
    /// </summary>
    Task<IEnumerable<RuleDto>> GetRules(CancellationToken token);
}