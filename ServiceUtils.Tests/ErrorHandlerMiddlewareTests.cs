using Microsoft.AspNetCore.Http;
using Moq;
using ServiceUtils.Exceptions;
using ServiceUtils.Midleware;

namespace ServiceUtils.Tests;

public class ErrorHandlerMiddlewareTests
{
    [Fact]
    public async Task NotFound()
    {
        Task Func(HttpContext contex)
        {
            throw new EntityNotException("1");
        }

        var item = new ErrorHandlerMiddleware(Func);

        var httpContext = new DefaultHttpContext();
        await item.InvokeAsync(httpContext);

        Assert.Equal(404, httpContext.Response.StatusCode);
    }

    [Fact]
    public async Task InvalidPropertyBadRequest()
    {
        Task Func(HttpContext contex)
        {
            throw new InvalidPropertyException("1")
            {
                PropertyName = "ID"
            };
        }

        var item = new ErrorHandlerMiddleware(Func);

        var httpContext = new DefaultHttpContext();
        await item.InvokeAsync(httpContext);

        Assert.Equal(400, httpContext.Response.StatusCode);
    }
    
    [Fact]
    public async Task EntityAlreadyExistBadRequest()
    {
        Task Func(HttpContext contex)
        {
            throw new EntityAlreadyExistException("1")
            {
                PropertyName = "ID"
            };
        }

        var item = new ErrorHandlerMiddleware(Func);

        var httpContext = new DefaultHttpContext();
        await item.InvokeAsync(httpContext);

        Assert.Equal(400, httpContext.Response.StatusCode);
    }
    
    [Fact]
    public async Task OtherExceptionBadRequest()
    {
        Task Func(HttpContext contex)
        {
            throw new Exception();
        }

        var item = new ErrorHandlerMiddleware(Func);

        var httpContext = new DefaultHttpContext();
        await item.InvokeAsync(httpContext);

        Assert.Equal(500, httpContext.Response.StatusCode);
    }
    
    [Fact]
    public async Task Ok()
    {
        Task Func(HttpContext contex)
        {
            return Task.CompletedTask;
        }

        var item = new ErrorHandlerMiddleware(Func);

        var httpContext = new DefaultHttpContext();
        await item.InvokeAsync(httpContext);

        Assert.Equal(200, httpContext.Response.StatusCode);
    }
}