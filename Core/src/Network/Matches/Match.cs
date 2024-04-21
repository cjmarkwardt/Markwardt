namespace Markwardt;

public interface IMatch
{
    string Id { get; }
    IReadOnlyDictionary<string, string> Properties { get; }

    IObservable Refreshed { get; }

    void Refresh();
}

public abstract class Match : IMatch
{
    public abstract string Id { get; }

    private readonly Dictionary<string, string> properties = [];
    public IReadOnlyDictionary<string, string> Properties => properties;

    private readonly Subject refreshed = new();
    public IObservable Refreshed => refreshed;

    public abstract void Refresh();

    protected void SetProperties(IEnumerable<KeyValuePair<string, string>> properties)
    {
        this.properties.Clear();
        foreach (KeyValuePair<string, string> property in properties)
        {
            this.properties[property.Key] = property.Value;
        }
        
        refreshed.OnNext();
    }
}