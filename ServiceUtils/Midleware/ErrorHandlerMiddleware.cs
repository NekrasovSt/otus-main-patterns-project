using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ServiceUtils.Dto;
using ServiceUtils.Interfaces;

namespace ServiceUtils.Midleware;

public class ErrorHandlerMiddleware
{
    // Holds the next middleware in the pipeline to invoke
    private readonly RequestDelegate _next;
    private readonly IServiceProvider _serviceProvider;

    // Constructor injects the next middleware and a logger
    public ErrorHandlerMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    // This method is called for every HTTP request. Handles errors during the request processing.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var handlers = _serviceProvider.GetServices<IExceptionHandler>();
            foreach (var handler in handlers)
            {
                var result = handler.HandleException(ex);
                if (result != null)
                {
                    context.Response.StatusCode = result.Code;
                    await context.Response.WriteAsJsonAsync(result.Error);
                    return;
                }
            }
            
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new ErrorDto() { Message = "Внутрення ошибка" });
        }
    }
}