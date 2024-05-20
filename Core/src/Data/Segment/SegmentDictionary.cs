namespace Markwardt;

public static class SegmentDictionary
{
    public static object Create(Type keyType, Type itemType, IDataSegmentTyper segmentTyper, IDataHandler handler, DataDictionary dictionary)
        => typeof(SegmentDictionary<,>).MakeGenericType(keyType, itemType).GetConstructor([typeof(IDataSegmentTyper), typeof(IDataHandler), typeof(DataDictionary)]).NotNull().Invoke([segmentTyper, handler, dictionary]);
}

public interface ISegmentDictionary<TKey, T> : IEnumerable<KeyValuePair<TKey, T>>
    where T : class
{
    int Count { get; }

    IEnumerable<TKey> Keys { get; }

    bool TryGetValue<TDerived>(TKey key, [MaybeNullWhen(false)] out TDerived value)
        where TDerived : class, T;

    TDerived Get<TDerived>(TKey key)
        where TDerived : class, T;
    
    TDerived Add<TDerived>(TKey key)
        where TDerived : class, T;

    void Remove(TKey key);

    void Clear();
}

public static class SegmentDictionaryExtensions
{
    public static bool TryGetValue<TKey, T>(this ISegmentDictionary<TKey, T> dictionary, TKey key, [MaybeNullWhen(false)] out T value)
        where T : class
        => dictionary.TryGetValue<T>(key, out value);

    public static T Get<TKey, T>(this ISegmentDictionary<TKey, T> dictionary, TKey key)
        where T : class
        => dictionary.Get<T>(key);

    public static T Add<TKey, T>(this ISegmentDictionary<TKey, T> dictionary, TKey key)
        where T : class
        => dictionary.Add<T>(key);
}

public class SegmentDictionary<TKey, T>(IDataSegmentTyper segmentTyper, IDataHandler handler, DataDictionary data) : ISegmentDictionary<TKey, T>
    where T : class
{
    private readonly DataKeyReader<TKey> keyReader = new(handler);
    private readonly DataSegmentReader<T> itemReader = new(segmentTyper, handler);

    public int Count => data.Count;
    public IEnumerable<TKey> Keys => data.Keys.Select(keyReader.Read);

    public bool TryGetValue<TDerived>(TKey key, [MaybeNullWhen(false)] out TDerived value)
        where TDerived : class, T
    {
        if (data.TryGetValue(keyReader.Write(key), out IDataNode? node) && segmentTyper.Is(node, typeof(TDerived)))
        {
            value = itemReader.Read<TDerived>(node);
            return true;
        }

        value = default;
        return false;
    }

    public TDerived Get<TDerived>(TKey key)
        where TDerived : class, T
        => TryGetValue<TDerived>(key, out TDerived? item) ? item : throw new InvalidOperationException($"Key {key} not found");

    public TDerived Add<TDerived>(TKey key)
        where TDerived : class, T
    {
        IDataNode segmentData = segmentTyper.Create(typeof(TDerived));
        data.Add(keyReader.Write(key), segmentData);
        return itemReader.Read<TDerived>(segmentData);
    }

    public void Remove(TKey key)
        => data.Remove(keyReader.Write(key));

    public void Clear()
        => data.Clear();

    public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        => data.Select(x => new KeyValuePair<TKey, T>(keyReader.Read(x.Key), itemReader.Read<T>(x.Value))).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}