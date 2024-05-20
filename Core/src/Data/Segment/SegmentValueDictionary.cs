namespace Markwardt;

public static class SegmentValueDictionary
{
    public static object Create(Type keyType, Type itemType, IDataHandler handler, DataDictionary dictionary)
        => typeof(SegmentValueDictionary<,>).MakeGenericType(keyType, itemType).GetConstructor([typeof(IDataHandler), typeof(DataDictionary)]).NotNull().Invoke([handler, dictionary]);
}

public class SegmentValueDictionary<TKey, T>(IDataHandler handler, DataDictionary data) : IDictionary<TKey, T>
{
    private readonly DataPairReader<TKey, T> reader = new(handler);

    public T this[TKey key] { get => reader.Items.Read(data[reader.Keys.Write(key)]); set => data[reader.Keys.Write(key)] = reader.Items.Write(value); }

    public ICollection<TKey> Keys => data.Keys.Select(reader.Keys.Read).AsCollection();
    public ICollection<T> Values => data.Values.Select(reader.Items.Read).AsCollection();

    public int Count => data.Count;
    public bool IsReadOnly => false;

    public void Add(TKey key, T value)
        => data.Add(reader.Keys.Write(key), reader.Items.Write(value));

    public void Add(KeyValuePair<TKey, T> item)
        => ((ICollection<KeyValuePair<string, IDataNode>>)data).Add(reader.Write(item));

    public void Clear()
        => data.Clear();

    public bool Contains(KeyValuePair<TKey, T> item)
        => data.Contains(reader.Write(item));

    public bool ContainsKey(TKey key)
        => data.ContainsKey(reader.Keys.Write(key));

    public void CopyTo(KeyValuePair<TKey, T>[] array, int arrayIndex)
    {
        foreach (KeyValuePair<string, IDataNode> pair in data)
        {
            array[arrayIndex++] = reader.Read(pair);
        }
    }

    public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        => data.Select(reader.Read).GetEnumerator();

    public bool Remove(TKey key)
        => data.Remove(reader.Keys.Write(key));

    public bool Remove(KeyValuePair<TKey, T> item)
        => ((ICollection<KeyValuePair<string, IDataNode>>)data).Remove(reader.Write(item));

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
    {
        if (data.TryGetValue(reader.Keys.Write(key), out IDataNode? node))
        {
            value = reader.Items.Read(node);
            return true;
        }

        value = default;
        return false;
    }

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
