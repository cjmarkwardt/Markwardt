namespace Markwardt;

public static class ObservableReadOnlyCacheDictionaryExtensions
{
    public static IReadOnlyDictionary<TKey, T> AsDictionary<TKey, T>(this IObservableReadOnlyCache<TKey, T> cache)
        where TKey : notnull
        where T : notnull
        => new ObservableReadOnlyCacheDictionary<TKey, T>(cache);
}

public class ObservableReadOnlyCacheDictionary<TKey, T>(IObservableReadOnlyCache<TKey, T> target) : IReadOnlyDictionary<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public T this[TKey key] => target.GetValue(key).Value;
    public IEnumerable<TKey> Keys => target.Keys;
    public IEnumerable<T> Values => target;
    public int Count => target.Count;

    public bool ContainsKey(TKey key)
        => target.ContainsKey(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        => target.TryGetValue(key, out value);

    public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
        => target.Pairs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}