using LinkServer.Dto;

namespace LinkServer.Services;

public interface IRuleEditorClient
{
    Task<IEnumerable<RuleDto>> GetRules(CancellationToken token);
}