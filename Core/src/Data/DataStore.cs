namespace Markwardt;

public interface IDataStore : IMultiDisposable
{
    ValueTask Save(IEnumerable<KeyValuePair<string, DataDictionary>> entries);
    ValueTask<IReadOnlyDictionary<string, DataDictionary>> Load(IEnumerable<string> ids);
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

    public ValueTask<IReadOnlyDictionary<string, DataDictionary>> Load(IEnumerable<string> ids)
        => ValueTask.FromResult<IReadOnlyDictionary<string, DataDictionary>>(ids.ToDictionary(x => x, x => entries[x]));
}