namespace Markwardt;

public class IdData(string id, string type, IEnumerable<KeyValuePair<string, object?>> properties)
{
    private IdData()
        : this(string.Empty, string.Empty, Enumerable.Empty<KeyValuePair<string, object?>>()) { }

    public string Id => id;
    public string Type => type;
    public IDictionary<string, object?> Properties { get; } = properties.ToDictionary(x => x.Key, x => x.Value);
}