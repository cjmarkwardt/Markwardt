namespace Markwardt;

public interface IDataStore
{
    ValueTask Save(IEnumerable<KeyValuePair<string, IDataNode>> nodes);
    IAsyncEnumerable<KeyValuePair<string, IDataNode>> Load(IEnumerable<string> keys);
}

public class DataStore(Stream stream, IDataExplorer explorer, IDataIndexSerializer indexSerializer) : IDataStore
{
    public async ValueTask Save(IEnumerable<KeyValuePair<string, IDataNode>> nodes)
    {
        await indexSerializer.Deserialize(explorer.GetStream(stream, 0));
    }

    public IAsyncEnumerable<KeyValuePair<string, IDataNode>> Load(IEnumerable<string> keys)
    {
        throw new NotImplementedException();
    }
}