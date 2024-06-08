namespace Markwardt;

public interface IDataStore : IMultiDisposable
{
    ValueTask Save(IEnumerable<KeyValuePair<string, DataDictionary>> entries);
    ValueTask<IDictionary<string, DataDictionary>> Load(IEnumerable<string> ids);
}

public class DataStore : ExtendedDisposable, IDataStore
{
    private readonly Dictionary<string, DataDictionary> entries = [];

    public ValueTask Save(IEnumerable<KeyValuePair<string, DataDictionary>> entries)
    {
        foreach (KeyValuePair<string, DataDictionary> field in entries)
        {
            this.entries[field.Key] = field.Value;
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask<IDictionary<string, DataDictionary>> Load(IEnumerable<string> ids)
        => ValueTask.FromResult<IDictionary<string, DataDictionary>>(ids.ToDictionary(x => x, x => entries[x]));
}