namespace LinkServer.Middleware;

public class EnrichMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    public const string ContextKey = "contextKey";

    public EnrichMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

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