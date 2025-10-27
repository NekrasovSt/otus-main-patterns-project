using System.Collections.ObjectModel;

namespace LinkServer;

public class QueryStringFiller: IFiller
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public const string QueryPrefix = "$q:";

    public QueryStringFiller(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

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