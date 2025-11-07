using System.Collections.ObjectModel;

namespace LinkServer;

/// <summary>
/// Добавление параметрой из queryString
/// </summary>
public class QueryStringFiller: IFiller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    /// <summary>
    /// Префикс заполнителя
    /// </summary>
    public const string QueryPrefix = "$q:";

    /// <summary>
    /// Конструктор
    /// </summary>
    public QueryStringFiller(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, object> Fill()
    {
        if (_httpContextAccessor?.HttpContext?.Request?.Query == null)
        {
            return new ReadOnlyDictionary<string, object>(new Dictionary<string, object>());
        }
        var dict = _httpContextAccessor.HttpContext.Request.Query.ToDictionary(i => $"{QueryPrefix}{i.Key}",
            i => (object)i.Value.ToString());

        return dict.AsReadOnly();
    }
}