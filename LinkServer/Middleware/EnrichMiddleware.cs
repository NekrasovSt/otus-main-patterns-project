namespace LinkServer.Middleware;

/// <summary>
/// Обогазение запроса данными
/// </summary>
public class EnrichMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Ключ в HTTP context
    /// </summary>
    public const string ContextKey = "contextKey";

    /// <summary>
    /// Конструктор
    /// </summary>
    public EnrichMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <summary>
    /// Обработка
    /// </summary>
    public async Task InvokeAsync(HttpContext context)
    {
        var result = new Dictionary<string, object>();
        using var scope = _serviceProvider.CreateScope();
        var fillers = scope.ServiceProvider.GetServices<IFiller>();
        foreach (var filler in fillers.SelectMany(i => i.Fill()))
        {
            result[filler.Key] =  filler.Value; 
        }
        context.Items[ContextKey] = result;
        await _next(context);
    }
}