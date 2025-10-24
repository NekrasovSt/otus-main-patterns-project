using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

bool IsMobileDevice(string userAgent)
{
    // List of common keywords in mobile user agent strings
    string[] mobileKeywords = { "Mobi", "Android", "iPhone", "iPad", "Windows Phone" };

    foreach (string keyword in mobileKeywords)
    {
        if (userAgent.Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }
    }

    return false;
}
string DetectOS(string userAgent)
{
    if (userAgent.Contains("Windows NT", StringComparison.OrdinalIgnoreCase))
    {
        if (userAgent.Contains("Windows Phone", StringComparison.OrdinalIgnoreCase))
        {
            return "Windows Phone";
        }

        if (userAgent.Contains("Win64", StringComparison.OrdinalIgnoreCase))
        {
            return "Windows 64-bit";
        }

        return "Windows 32-bit";
    }

    if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
    {
        return "macOS";
    }

    if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
    {
        return "Android";
    }

    if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
    {
        return "iOS (iPhone)";
    }

    if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
    {
        return "iOS (iPad)";
    }

    return "Unknown";
}

app.MapGet("/link", (HttpContext context) =>
    {
        var userAgent = context.Request.Headers.ContainsKey(HeaderNames.UserAgent)
            ? context.Request.Headers.UserAgent.ToString().ToLower()
            : null;

        var browser = "Unknown";
        if (string.IsNullOrEmpty(browser))
        {
            return browser;
        }

        var acceptLanguage = context.Request.Headers.ContainsKey(HeaderNames.AcceptLanguage)
            ? context.Request.Headers.AcceptLanguage.ToString()
            : null;
        var languages = acceptLanguage
            .Split(',')
            .Select(lang =>
            {
                var parts = lang.Split(';');
                var language = parts[0].Trim();
                var quality = 1.0; // значение по умолчанию
            
                if (parts.Length > 1 && parts[1].Trim().StartsWith("q="))
                {
                    _ = double.TryParse(parts[1].Trim().Substring(2), out quality);
                }
            
                return new { Language = language, Quality = quality };
            })
            .OrderByDescending(lang => lang.Quality)
            .Select(lang => lang.Language)
            .ToList();
        
        var isMobile = IsMobileDevice(userAgent);
        var os = DetectOS(userAgent);


        if (userAgent.Contains("chrome") && !userAgent.Contains("edg"))
            browser = "Chrome";
        else if (userAgent.Contains("firefox"))
            browser = "Firefox";
        else if (userAgent.Contains("safari") && !userAgent.Contains("chrome"))
            browser = "Safari";
        else if (userAgent.Contains("edg"))
            browser = "Edge";
        else if (userAgent.Contains("opera"))
            browser = "Opera";
        return browser;
    })
    .WithName("Link")
    .WithOpenApi();

app.Run();