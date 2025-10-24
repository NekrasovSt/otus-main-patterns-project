namespace LinkServer;

public class IpFiller : IFiller
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public const string Address = "adress";

    public IpFiller(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public IReadOnlyDictionary<string, object> Fill()
    {
        var dict = new Dictionary<string, object>();
        var address = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

        dict.Add(Address, address);
        return dict.AsReadOnly();
    }
}