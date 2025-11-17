using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using ServiceUtils.Exceptions;
using ServiceUtils.Handlers;
using ServiceUtils.Interfaces;
using ServiceUtils.Midleware;

namespace ServiceUtils.Tests;

public class ErrorHandlerMiddlewareTests
{
    private IServiceProvider GetServiceProvider()
    {
        var collection = new ServiceCollection();

        collection.AddTransient<IExceptionHandler, EntityAlreadyExistExceptionHandler>();
        collection.AddTransient<IExceptionHandler, EntityNotFoundExceptionHandler>();
        collection.AddTransient<IExceptionHandler, InvalidPropertyExceptionHandler>();
        return collection.BuildServiceProvider();
    }

    [Fact]
    public async Task NotFound()
    {
        Task Func(HttpContext contex)
        {
            throw new EntityNotFoundException("1");
        }

        var serviceProvider = GetServiceProvider();
        var item = new ErrorHandlerMiddleware(Func, serviceProvider);

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

        var serviceProvider = GetServiceProvider();
        var item = new ErrorHandlerMiddleware(Func, serviceProvider);

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

        var serviceProvider = GetServiceProvider();
        var item = new ErrorHandlerMiddleware(Func, serviceProvider);

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

        var serviceProvider = GetServiceProvider();
        var item = new ErrorHandlerMiddleware(Func, serviceProvider);

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

        var serviceProvider = GetServiceProvider();
        var item = new ErrorHandlerMiddleware(Func, serviceProvider);

        var httpContext = new DefaultHttpContext();
        await item.InvokeAsync(httpContext);

        Assert.Equal(200, httpContext.Response.StatusCode);
    }

    [Fact]
    public void NullArgumentExceptionDelegate()
    {
        var serviceProvider = GetServiceProvider();
        Assert.Throws<ArgumentNullException>(() => new ErrorHandlerMiddleware(null!, serviceProvider));
    }

    [Fact]
    public void NullArgumentExceptionProvider()
    {
        Task Func(HttpContext contex)
        {
            return Task.CompletedTask;
        }

        Assert.Throws<ArgumentNullException>(() => new ErrorHandlerMiddleware(Func, null!));
    }
}