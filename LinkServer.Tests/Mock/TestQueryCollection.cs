using System.Collections;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace LinkServer.Tests;

public class TestQueryCollection : IQueryCollection
{
    private readonly Dictionary<string, StringValues> _values;

    public TestQueryCollection(Dictionary<string, StringValues> values)
    {
        _values = values;
    }

    public StringValues this[string key] => _values.ContainsKey(key) ? _values[key] : StringValues.Empty;

    public int Count => _values.Count;

    public ICollection<string> Keys => _values.Keys;

    public bool ContainsKey(string key) => _values.ContainsKey(key);

    public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator() => _values.GetEnumerator();

    public bool TryGetValue(string key, out StringValues value) => _values.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}