using System.Net;
using Microsoft.AspNetCore.Http;
using Moq;

namespace LinkServer.Tests;

public class IpFillerTests
{
    [Fact]
    public void Unknown()
    {
        var accessor = new Mock<IHttpContextAccessor>();
        var filler = new IpFiller(accessor.Object);
        var dictionary = filler.Fill();
        
        Assert.Equal("Unknown", dictionary[IpFiller.Address]);
    }
    
    [Fact]
    public void RealIp()
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(i => i.HttpContext!.Connection.RemoteIpAddress)
            .Returns(new IPAddress(new byte[] { 192, 168, 1, 1 }));
        var filler = new IpFiller(accessor.Object);
        var dictionary = filler.Fill();
        
        Assert.Equal("192.168.1.1", dictionary[IpFiller.Address]);
    }
    
    [Fact]
    public void NullArgument()
    {
        Assert.Throws<ArgumentNullException>(()=> new IpFiller(null!));
    }
}