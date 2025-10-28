using System.Net;
using RuleEditor.Dto;
using RuleEditor.Exceptions;

namespace RuleEditor.Midleware;

public class ErrorHandlerMiddleware
{
    // Holds the next middleware in the pipeline to invoke
    private readonly RequestDelegate _next;

    // Constructor injects the next middleware and a logger
    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    // This method is called for every HTTP request. Handles errors during the request processing.
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (EntityNotException e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(new ErrorDto() { Id = e.Id, Message = e.Message });
        }
        catch (InvalidPropertyException e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorDto() { Id = e.Id, Message = e.Message });
        }
        catch (EntityAlreadyExistException e)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new ErrorDto() { Message = e.Message });
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new ErrorDto() { Message = "Внутрення ошибка" });
        }
    }
}