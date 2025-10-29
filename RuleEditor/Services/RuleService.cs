using System.Validation;
using RuleEditor.Exceptions;
using RuleEditor.Interface;
using RuleEditor.Models;
using RuleExecutor;

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

    public async Task<Rule> UpdateAsync(Rule rule, CancellationToken token)
    {
        await ValidateIdAsync(rule.Id, token);
        await ValidateIdAsync(rule, token);
        return await _ruleRepository.UpdateAsync(rule, token);
    }

    private async Task ValidateIdAsync(Rule rule, CancellationToken token)
    {
        var result = new RuleValidator().Validate(rule);
        if (!result)
        {
            var error = result.Errors.FirstOrDefault();
            throw new InvalidPropertyException(error.ErrorMessage)
            {
                PropertyName = nameof(error.PropertyName)
            };
        }

        var conditionValidator = new FilterConditionValidator();
        result = Validate(conditionValidator, rule.FilterCondition);
        if (!result)
        {
            var error = result.Errors.FirstOrDefault();
            throw new InvalidPropertyException(error.ErrorMessage)
            {
                PropertyName = nameof(error.PropertyName)
            };
        }
    }

    private FlatValidationResult Validate(in FilterConditionValidator conditionValidator, in FilterCondition condition)
    {
        FlatValidationResult result = conditionValidator.Validate(condition);
        if (!result)
        {
            return result;
        }

        foreach (var filterCondition in condition.Conditions)
        {
            result = conditionValidator.Validate(filterCondition);
            if (!result)
            {
                return result;
            }
        }

        return result;
    }

    private async Task ValidateIdAsync(string id, CancellationToken token)
    {
        var result =
            await FlatValidator.ValidateAsync(id, v =>
                {
                    v.ValidIf(m => m.Length == 24, "Invalid id", m => m);
                    v.ValidIf(m => m.All("0123456789abcdefABCDEF".Contains), "Invalid id", m => m);
                    v.ErrorIf(m => m.IsEmpty(), "Empty id", m => m);
                },
                token);
        if (!result)
        {
            throw new InvalidPropertyException("Параметр Id должен быть длинной 24 hex символа")
            {
                PropertyName = nameof(id)
            };
        }
    }
}