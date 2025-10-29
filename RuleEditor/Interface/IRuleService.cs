using RuleEditor.Models;

namespace RuleEditor.Interface;

public interface IRuleService
{
    Task<IEnumerable<Rule>> GetAllAsync(CancellationToken token);
    
    Task<Rule> GetAsync(string id, CancellationToken token);
    
    Task DeleteAsync(string id, CancellationToken token);
    Task<Rule> AddAsync(Rule newRule, CancellationToken token);
    Task<Rule> UpdateAsync(Rule rule, CancellationToken token);
}