using LinkServer.Dto;

namespace LinkServer.Services;

/// <inheritdoc />
public class RuleEditorClient : IRuleEditorClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Конструктор
    /// </summary>
    public RuleEditorClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<RuleDto>> GetRules(CancellationToken token)
    {
        using var client = _httpClientFactory.CreateClient("rule-editor");
        var result = await client.GetAsync("/rules/all", cancellationToken: token);
        result.EnsureSuccessStatusCode();
        var response = await result.Content.ReadFromJsonAsync<IEnumerable<RuleDto>>(cancellationToken: token);
        return response ?? [];
    }
}