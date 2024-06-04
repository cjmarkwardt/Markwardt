namespace Markwardt;

public class HandledDictionary<TKey, T>(HandledDictionary<TKey, T>.IHandler handler) : IDictionary<TKey, T>, IReadOnlyDictionary<TKey, T>
    where TKey : notnull
{
    private readonly Dictionary<TKey, T> dictionary = [];

    private ICollection<KeyValuePair<TKey, T>> Collection => dictionary;

    public T this[TKey key]
    {
        get => dictionary[key];
        set
        {
            dictionary[key] = value;
            handler.OnSet(key, value);
        }
    }

    public ICollection<TKey> Keys => dictionary.Keys;
    public ICollection<T> Values => dictionary.Values;

    public int Count => dictionary.Count;
    public bool IsReadOnly => false;

    IEnumerable<TKey> IReadOnlyDictionary<TKey, T>.Keys => Keys;
    IEnumerable<T> IReadOnlyDictionary<TKey, T>.Values => Values;

    public void Add(TKey key, T value)
    {
        dictionary.Add(key, value);
        handler.OnAdd(key, value);
    }

    public void Add(KeyValuePair<TKey, T> item)
    {
        Collection.Add(item);
        handler.OnAdd(item.Key, item.Value);
    }

    public void Clear()
    {
        dictionary.Clear();
        handler.OnClear();
    }

    public bool Contains(KeyValuePair<TKey, T> item)
        => Collection.Contains(item);

    public bool ContainsKey(TKey key)
        => dictionary.ContainsKey(key);

    public void CopyTo(KeyValuePair<TKey, T>[] array, int arrayIndex)
        => Collection.CopyTo(array, arrayIndex);

    public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        => dictionary.GetEnumerator();

    public bool Remove(TKey key)
    {
        if (dictionary.TryGetValue(key, out T? value))
        {
            dictionary.Remove(key);
            handler.OnRemove(key, value);
            return true;
        }

        return false;
    }

    public bool Remove(KeyValuePair<TKey, T> item)
    {
        if (Collection.Remove(item))
        {
            handler.OnRemove(item.Key, item.Value);
            return true;
        }

        return false;
    }

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        => dictionary.TryGetValue(key, out value);

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    public interface IHandler
    {
        void OnAdd(TKey key, T value);
        void OnSet(TKey key, T value);
        void OnRemove(TKey key, T value);
        void OnClear();
    }
}