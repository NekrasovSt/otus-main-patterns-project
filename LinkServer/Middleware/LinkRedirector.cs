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
        if (!_memoryCache.TryGetValue("rules", out IEnumerable<RuleDto>? rules))
        {
            rules = await _ruleEditorClient.GetRules(token);
            _memoryCache.Set("rules", rules);
        }


        var builder = new DictionaryExpressionBuilder();
        foreach (var rule in rules)
        {
            var expression = builder.BuildExpression(rule.FilterCondition.Adapt<FilterCondition>());

            if (expression.Compile().Invoke(dictionary))
            {
                return rule.Link;
            }
        }
        
        return DefaultLink;
    }
}