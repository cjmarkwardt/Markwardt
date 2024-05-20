namespace Markwardt;

public class DataPairReader<TKey, T>(IDataHandler handler)
{
    public DataKeyReader<TKey> Keys { get; } = new(handler);
    public DataReader<T> Items { get; } = new(handler);
    
    public KeyValuePair<TKey, T> Read(KeyValuePair<string, IDataNode> data)
        => new(Keys.Read(data.Key), Items.Read(data.Value));

    public KeyValuePair<string, IDataNode> Write(KeyValuePair<TKey, T> pair)
        => new(Keys.Write(pair.Key), Items.Write(pair.Value));
}