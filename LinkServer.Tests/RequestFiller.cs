using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Moq;

namespace LinkServer.Tests;

public class RequestFillerTests
{
    [Theory]
    [InlineData(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36",
        false)]
    [InlineData(
        "Mozilla/5.0 (Linux; Android 11; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.62 Mobile Safari/537.36",
        true)]
    [InlineData(
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_4) AppleWebKit/600.7.12 (KHTML, like Gecko) Version/8.0.7 Safari/600.7.12",
        false)]
    [InlineData(
        "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1",
        true)]
    public void IsMobile(string userAgent, bool resultValue)
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(i => i.HttpContext.Request.Headers.ContainsKey(HeaderNames.UserAgent)).Returns(true);
        accessor.Setup(i => i.HttpContext.Request.Headers[HeaderNames.UserAgent]).Returns(userAgent);

        var service = new RequestFiller(accessor.Object);
        var result = service.Fill();
        Assert.Equal(resultValue, result[RequestFiller.IsMobile]);
    }

    [Theory]
    [InlineData(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36",
        "Windows 64-bit")]
    [InlineData(
        "Mozilla/5.0 (Linux; Android 11; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.62 Mobile Safari/537.36",
        "Android")]
    [InlineData(
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_4) AppleWebKit/600.7.12 (KHTML, like Gecko) Version/8.0.7 Safari/600.7.12",
        "macOS")]
    [InlineData(
        "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1",
        "iOS (iPhone)")]
    [InlineData(
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
        "Linux")]
    public void Os(string userAgent, string os)
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(i => i.HttpContext.Request.Headers.ContainsKey(HeaderNames.UserAgent)).Returns(true);
        accessor.Setup(i => i.HttpContext.Request.Headers[HeaderNames.UserAgent]).Returns(userAgent);

        var service = new RequestFiller(accessor.Object);
        var result = service.Fill();
        Assert.Equal(os, result[RequestFiller.Os]);
    }

    [Theory]
    [InlineData(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36",
        "Chrome")]
    [InlineData(
        "Mozilla/5.0 (Linux; Android 11; Pixel 5) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.62 Mobile Safari/537.36",
        "Chrome")]
    [InlineData(
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_4) AppleWebKit/600.7.12 (KHTML, like Gecko) Version/8.0.7 Safari/600.7.12",
        "Safari")]
    [InlineData(
        "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3 like Mac OS X) AppleWebKit/602.1.50 (KHTML, like Gecko) CriOS/56.0.2924.75 Mobile/14E5239e Safari/602.1",
        "Chrome")]
    [InlineData(
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
        "Chrome")]
    [InlineData("Opera/9.80 (X11; Linux i686; Ubuntu/14.10) Presto/2.12.388 Version/12.16.2", "Opera")]
    [InlineData(
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36 Edg/91.0.864.59",
        "Edge")]
    public void Browser(string userAgent, string browser)
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(i => i.HttpContext.Request.Headers.ContainsKey(HeaderNames.UserAgent)).Returns(true);
        accessor.Setup(i => i.HttpContext.Request.Headers[HeaderNames.UserAgent]).Returns(userAgent);

        var service = new RequestFiller(accessor.Object);
        var result = service.Fill();
        Assert.Equal(browser, result[RequestFiller.Browser]);
    }

    [Theory]
    [InlineData("en-US, en;q=0.9, fr-FR;q=0.8, fr;q=0.7", "en-US", "en-US,en,fr-FR,fr")]
    [InlineData("da, en-gb;q=0.8, en;q=0.7", "da", "da,en-gb,en")]
    public void Language(string acceptLanguage, string language, string all)
    {
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(i => i.HttpContext.Request.Headers.ContainsKey(HeaderNames.AcceptLanguage)).Returns(true);
        accessor.Setup(i => i.HttpContext.Request.Headers[HeaderNames.AcceptLanguage]).Returns(acceptLanguage);

        var service = new RequestFiller(accessor.Object);
        var result = service.Fill();
        Assert.Equal(language, result[RequestFiller.PreferredLanguage]);
        Assert.Equal(all, result[RequestFiller.AllLanguages]);
    }
}