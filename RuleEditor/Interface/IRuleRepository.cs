using RuleEditor.Models;

namespace RuleEditor.Interface;

public interface IRuleRepository
{
    public Task<IEnumerable<Rule>> GetAllAsync(CancellationToken token);
    public Task<Rule> GetAsync(string id, CancellationToken token);
    public Task DeleteAsync(string id, CancellationToken token);
    public Task<Rule> AddAsync(Rule newRule, CancellationToken token);
    public Task InitDBAsync();
}