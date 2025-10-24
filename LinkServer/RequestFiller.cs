using System.Globalization;
using Microsoft.Net.Http.Headers;

namespace LinkServer;

public class RequestFiller: IFiller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public const string Browser = "browser";
    public const string IsMobile = "isMobile";
    public const string Os = "os";
    public const string PreferredLanguage = "preferredLanguage";
    public const string AllLanguages = "allLanguages";

    public RequestFiller(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public IReadOnlyDictionary<string, object> Fill()
    {
        var dict = new Dictionary<string, object>();

        var userAgent = _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(HeaderNames.UserAgent)
            ? _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.UserAgent].ToString()
            : null;

        FillBrowser(dict, userAgent);
        FillMobile(dict, userAgent);
        FillOs(dict, userAgent);

        var acceptLanguage = _httpContextAccessor.HttpContext.Request.Headers.ContainsKey(HeaderNames.AcceptLanguage)
            ? _httpContextAccessor.HttpContext.Request.Headers[HeaderNames.AcceptLanguage].ToString()
            : null;

        FillLanguage(dict, acceptLanguage);
        return dict.AsReadOnly();
    }

    private void FillOs(Dictionary<string, object> dict, string? userAgent)
    {
        if (string.IsNullOrEmpty(userAgent))
        {
            dict.Add(Os, "Unknown");
            return;
        }

        if (userAgent.Contains("Windows NT", StringComparison.OrdinalIgnoreCase))
        {
            if (userAgent.Contains("Windows Phone", StringComparison.OrdinalIgnoreCase))
            {
                dict.Add(Os, "Windows Phone");
                return;
            }

            if (userAgent.Contains("Win64", StringComparison.OrdinalIgnoreCase))
            {
                dict.Add(Os, "Windows 64-bit");
                return;
            }

            dict.Add(Os, "Windows 32-bit");
            return;
        }

        if (userAgent.Contains("Linux x86_64", StringComparison.OrdinalIgnoreCase))
        {
            dict.Add(Os, "Linux");
            return;
        }
        
        if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase))
        {
            dict.Add(Os, "macOS");
            return;
        }

        if (userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase))
        {
            dict.Add(Os, "Android");
            return;
        }

        if (userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase))
        {
            dict.Add(Os, "iOS (iPhone)");
            return;
        }

        if (userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase))
        {
            dict.Add(Os, "iOS (iPad)");
            return;
        }
    }

    private void FillBrowser(Dictionary<string, object> dict, string? userAgent)
    {
        var browser = "Unknown";
        if (!string.IsNullOrEmpty(userAgent))
        {
            if (userAgent.Contains("chrome", StringComparison.InvariantCultureIgnoreCase) && !userAgent.Contains("edg", StringComparison.InvariantCultureIgnoreCase))
                browser = "Chrome";
            if (userAgent.Contains("crios", StringComparison.InvariantCultureIgnoreCase))
                browser = "Chrome";
            else if (userAgent.Contains("firefox", StringComparison.InvariantCultureIgnoreCase))
                browser = "Firefox";
            else if (userAgent.Contains("safari", StringComparison.InvariantCultureIgnoreCase) && !userAgent.Contains("chrome", StringComparison.InvariantCultureIgnoreCase))
                browser = "Safari";
            else if (userAgent.Contains("edg", StringComparison.InvariantCultureIgnoreCase))
                browser = "Edge";
            else if (userAgent.Contains("opera", StringComparison.InvariantCultureIgnoreCase))
                browser = "Opera";
        }

        dict.Add(Browser, browser);
    }

    private void FillMobile(Dictionary<string, object> dict, string? userAgent)
    {
        if (!string.IsNullOrEmpty(userAgent))
        {
            string[] mobileKeywords = { "Mobi", "Android", "iPhone", "iPad", "Windows Phone" };

            foreach (string keyword in mobileKeywords)
            {
                if (userAgent.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    dict.Add(IsMobile, true);
                    return;
                }
            }
        }

        dict.Add(IsMobile, false);
    }

    private void FillLanguage(Dictionary<string, object> dict, string? acceptLanguage)
    {
        if (string.IsNullOrEmpty(acceptLanguage))
        {
            dict.Add(PreferredLanguage, "ru");
            dict.Add(AllLanguages, "ru");
            return;
        }

        var languages = acceptLanguage
            .Split(',')
            .Select(lang =>
            {
                var parts = lang.Split(';');
                var language = parts[0].Trim();
                var quality = 1.0; // значение по умолчанию

                if (parts.Length > 1 && parts[1].Trim().StartsWith("q="))
                {
                    _ = double.TryParse(parts[1].Trim()[2..], CultureInfo.InvariantCulture, out quality);
                }

                return new { Language = language, Quality = quality };
            })
            .OrderByDescending(lang => lang.Quality)
            .Select(lang => lang.Language);
           
        dict.Add(PreferredLanguage, languages.First());
        dict.Add(AllLanguages, string.Join(',', languages));
    }
}