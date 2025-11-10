using LinkServer.Dto;
using LinkServer.Services;
using Mapster;
using MassTransit;
using Microsoft.Extensions.Caching.Memory;
using RuleExecutor;
using ServiceUtils.Broker;

namespace LinkServer.Middleware;

/// <inheritdoc />
public class LinkRedirector : ILinkRedirector
{
    private readonly IRuleEditorClient _ruleEditorClient;
    private readonly IMemoryCache _memoryCache;
    private readonly IPublishEndpoint _sendEndpointProvider;

    /// <summary>
    /// Ссылка по умолчанию если не одно правило не сработало
    /// </summary>
    public const string DefaultLink = "https://otus.ru";

    /// <summary>
    /// Конструктор
    /// </summary>
    public LinkRedirector(IRuleEditorClient ruleEditorClient, IMemoryCache memoryCache, IPublishEndpoint sendEndpointProvider)
    {
        _ruleEditorClient = ruleEditorClient ?? throw new ArgumentNullException(nameof(ruleEditorClient));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        _sendEndpointProvider = sendEndpointProvider ?? throw new ArgumentNullException(nameof(sendEndpointProvider));
    }

    /// <inheritdoc />
    public async Task<string> RedirectAsync(Dictionary<string, object> dictionary, CancellationToken token)
    {
        if (!_memoryCache.TryGetValue("rules", out IEnumerable<CompiledRule>? compiledRules) || compiledRules == null)
        {
            var builder = new DictionaryExpressionBuilder();
            var rules = await _ruleEditorClient.GetRules(token);
            compiledRules = rules.Select(rule => new CompiledRule()
            {
                Id = rule.Id,
                Link = rule.Link,
                Name = rule.Name,
                Predicate = builder.BuildExpression(rule.FilterCondition.Adapt<FilterCondition>()).Compile(),
            }).ToList();

            using var entry = _memoryCache.CreateEntry("rules");
            entry.Value = compiledRules;
        }

        foreach (var rule in compiledRules)
        {
            if (rule.Predicate.Invoke(dictionary))
            {
                await _sendEndpointProvider.Publish(new RuleExecutedDto()
                {
                    Date = DateTime.UtcNow,
                    RuleId = rule.Id,
                    RuleName = rule.Name,
                    Url = rule.Link
                }, token);
                return rule.Link;
            }
        }

        await _sendEndpointProvider.Publish(new RuleExecutedDto()
        {
            Date = DateTime.UtcNow,
            RuleId = "",
            RuleName = "Default",
            Url = DefaultLink
        }, token);
        return DefaultLink;
    }
}