using System.Validation;
using RuleEditor.Exceptions;
using RuleEditor.Interface;
using RuleEditor.Models;

namespace RuleEditor.Services;

public class RuleService : IRuleService
{
    private readonly IRuleRepository _ruleRepository;

    public RuleService(IRuleRepository ruleRepository)
    {
        _ruleRepository = ruleRepository;
    }

    public Task<IEnumerable<Rule>> GetAllAsync(CancellationToken token)
    {
        return _ruleRepository.GetAllAsync(token);
    }

    public async Task<Rule> GetAsync(string id, CancellationToken token)
    {
        await ValidateIdAsync(id, token);
        return await _ruleRepository.GetAsync(id, token);
    }

    public async Task DeleteAsync(string id, CancellationToken token)
    {
        await ValidateIdAsync(id, token);
        await _ruleRepository.DeleteAsync(id, token);
    }

    public async Task<Rule> AddAsync(Rule newRule, CancellationToken token)
    {
        newRule.Id = null;
        await ValidateIdAsync(newRule, token);
        return await _ruleRepository.AddAsync(newRule, token);
    }

    private async Task ValidateIdAsync(Rule rule, CancellationToken token)
    {
        var result =
            await FlatValidator.ValidateAsync(rule, v =>
                {
                    v.ErrorIf(m => m.Link.IsEmpty(), $"Поле {nameof(Rule.Link)} не может быть пустым", m => m.Link);
                    v.ErrorIf(m => m.Name.IsEmpty(), $"Поле {nameof(Rule.Name)} не может быть пустым", m => m.Name);
                },
                token);
        if (!result)
        {
            var error = result.Errors.FirstOrDefault();
            throw new InvalidPropertyException(error.ErrorMessage)
            {
                PropertyName = nameof(error.PropertyName)
            };
        }
    }

    private async Task ValidateIdAsync(string id, CancellationToken token)
    {
        var result =
            await FlatValidator.ValidateAsync(id, v =>
                {
                    v.ValidIf(m => m.Length == 24, "Invalid id", m => m);
                    v.ErrorIf(m => m.IsEmpty(), "Empty id", m => m);
                },
                token);
        if (!result)
        {
            throw new InvalidPropertyException("Параметр Id должен быть длинной 24 смивола")
            {
                PropertyName = nameof(id)
            };
        }
    }
}