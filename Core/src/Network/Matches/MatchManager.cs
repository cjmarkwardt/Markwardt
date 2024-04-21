namespace Markwardt;

public interface IMatchManager : IMultiDisposable
{
    string Id { get; }
    IReadOnlyDictionary<string, string> Properties { get; }

    void SetProperty(string key, string? value);
}

public abstract class MatchManager : ExtendedDisposable, IMatchManager
{
    private readonly Dictionary<string, string> properties = [];
    public IReadOnlyDictionary<string, string> Properties => properties;

    public abstract string Id { get; }

    public void SetProperty(string key, string? value)
    {
        if (value is null)
        {
            properties.Remove(key);
        }
        else
        {
            properties[key] = value;
        }

        OnPropertySet(key, value);
    }

    protected abstract void OnPropertySet(string key, string? value);
}