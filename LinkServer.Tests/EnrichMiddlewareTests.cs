using LinkServer.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace LinkServer.Tests;

public class EnrichMiddlewareTests
{
    [Fact]
    public async Task Fill()
    {
        Task Func(HttpContext contex)
        {
            return Task.CompletedTask;
        }

        var sc = new ServiceCollection();
        var filler = new Mock<IFiller>();
        filler.Setup(f => f.Fill()).Returns(new Dictionary<string, object>() { { "key", "value" } });
        sc.AddScoped<IFiller>(i => filler.Object);
        var middleware = new EnrichMiddleware(Func, sc.BuildServiceProvider());
        var httpContext = new DefaultHttpContext();
        await middleware.InvokeAsync(httpContext);

        Assert.IsAssignableFrom<Dictionary<string, object>>(httpContext.Items[EnrichMiddleware.ContextKey]);
    }

    [Fact]
    public void NullArgumentFunc()
    {
        var sc = new ServiceCollection();
        Assert.Throws<ArgumentNullException>(() => new EnrichMiddleware(null!, sc.BuildServiceProvider()));
    }

    [Fact]
    public void NullArgumentColl()
    {
        Task Func(HttpContext contex)
        {
            return Task.CompletedTask;
        }

        Assert.Throws<ArgumentNullException>(() => new EnrichMiddleware(Func, null!));
    }
}