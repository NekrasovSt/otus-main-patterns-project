namespace LinkServer;

/// <summary>
/// Заполняет IP клиента
/// </summary>
public class IpFiller : IFiller
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Адрес
    /// </summary>
    public const string Address = "adress";

    /// <summary>
    /// Конструктор
    /// </summary>
    public IpFiller(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object> Fill()
    {
        var dict = new Dictionary<string, object>();
        var address = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "Unknown";

        dict.Add(Address, address);
        return dict.AsReadOnly();
    }
}