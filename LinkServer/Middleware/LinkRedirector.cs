using LinkServer.Dto;
using LinkServer.Services;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using RuleExecutor;

namespace LinkServer.Middleware;

public class LinkRedirector: ILinkRedirector
{
    private readonly IRuleEditorClient _ruleEditorClient;
    private readonly IMemoryCache _memoryCache;

    public const string DefaultLink = "https://otus.ru";
    public LinkRedirector(IRuleEditorClient ruleEditorClient, IMemoryCache memoryCache)
    {
        _ruleEditorClient = ruleEditorClient ?? throw new ArgumentNullException(nameof(ruleEditorClient));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public async Task<string> RedirectAsync(Dictionary<string, object> dictionary, CancellationToken token)
    {
        if (!_memoryCache.TryGetValue("rules", out IEnumerable<CompiledRule>? compiledRules))
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
                return rule.Link;
            }
        }
        
        return DefaultLink;
    }
}