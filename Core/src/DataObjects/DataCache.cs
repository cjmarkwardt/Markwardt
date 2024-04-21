namespace Markwardt;

public interface IDataCache<TKey, TValue> : ICollection<TValue>
{
    TValue this[TKey key] { get; }

    TKey GetKey(TValue value);
    bool RemoveKey(TKey key);
    bool ContainsKey(TKey key);
    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
}

public static class DataCacheExtensions
{
    public static TValue PopKey<TKey, TValue>(this IDataCache<TKey, TValue> cache, TKey key)
    {
        TValue value = cache[key];
        cache.RemoveKey(key);
        return value;
    }
}

public class DataCache<TKey, TValue>(Func<TValue, TKey> getKey) : IDataCache<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> values = [];

    public int Count => values.Count;

    bool ICollection<TValue>.IsReadOnly => false;

    public TValue this[TKey key] => values[key];

    public TKey GetKey(TValue value)
        => getKey(value);

    public bool RemoveKey(TKey key)
        => values.Remove(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        => values.TryGetValue(key, out value);

    public void Add(TValue item)
        => values.Add(GetKey(item), item);

    public void Clear()
        => values.Clear();

    public bool Contains(TValue item)
        => values.ContainsKey(GetKey(item));

    public bool ContainsKey(TKey key)
        => values.ContainsKey(key);

    public void CopyTo(TValue[] array, int arrayIndex)
    {
        foreach (TValue value in this)
        {
            array[arrayIndex++] = value;
        }
    }

    public bool Remove(TValue item)
        => values.Remove(GetKey(item));

    public IEnumerator<TValue> GetEnumerator()
        => values.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}