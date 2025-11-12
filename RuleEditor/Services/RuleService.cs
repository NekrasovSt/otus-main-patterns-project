using System.Validation;
using MassTransit;
using RuleEditor.Interface;
using RuleEditor.Models;
using RuleExecutor;
using ServiceUtils.Broker;
using ServiceUtils.Exceptions;

namespace RuleEditor.Services;

/// <inheritdoc />
public class RuleService : IRuleService
{
    private readonly IRuleRepository _ruleRepository;
    private readonly IPublishEndpoint _sendEndpointProvider;

    /// <summary>
    /// Конструктор
    /// </summary>
    public RuleService(IRuleRepository ruleRepository, IPublishEndpoint sendEndpointProvider)
    {
        _ruleRepository = ruleRepository ?? throw new ArgumentNullException(nameof(ruleRepository));
        _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
    }

    /// <inheritdoc />
    public Task<IEnumerable<Rule>> GetAllAsync(CancellationToken token)
    {
        return _ruleRepository.GetAllAsync(token);
    }

    /// <inheritdoc />
    public async Task<Rule> GetAsync(string id, CancellationToken token)
    {
        await ValidateIdAsync(id, token);
        return await _ruleRepository.GetAsync(id, token);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string id, CancellationToken token)
    {
        await ValidateIdAsync(id, token);
        await _ruleRepository.DeleteAsync(id, token);
        await _sendEndpointProvider.Publish(new RuleChangedDto(), token);
    }

    /// <inheritdoc />
    public async Task<Rule> AddAsync(Rule newRule, CancellationToken token)
    {
        await ValidateIdAsync(newRule, token);
        await _sendEndpointProvider.Publish(new RuleChangedDto(), token);
        return await _ruleRepository.AddAsync(newRule, token);
    }

    /// <inheritdoc />
    public async Task<Rule> UpdateAsync(Rule rule, CancellationToken token)
    {
        await ValidateIdAsync(rule.Id, token);
        await ValidateIdAsync(rule, token);
        await _sendEndpointProvider.Publish(new RuleChangedDto(), token);
        return await _ruleRepository.UpdateAsync(rule, token);
    }

    private Task ValidateIdAsync(Rule rule, CancellationToken token)
    {
        var result = new RuleValidator().Validate(rule);
        if (!result)
        {
            var error = result.Errors.First();
            throw new InvalidPropertyException(error.ErrorMessage)
            {
                PropertyName = nameof(error.PropertyName)
            };
        }

        var conditionValidator = new FilterConditionValidator();
        result = Validate(conditionValidator, rule.FilterCondition);
        if (!result)
        {
            var error = result.Errors.First();
            throw new InvalidPropertyException(error.ErrorMessage)
            {
                PropertyName = nameof(error.PropertyName)
            };
        }

        return Task.CompletedTask;
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

    private async Task ValidateIdAsync(string? id, CancellationToken token)
    {
        if (id == null)
            throw new InvalidPropertyException("Параметр Id не может быть пустым")
            {
                PropertyName = nameof(id)
            };
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