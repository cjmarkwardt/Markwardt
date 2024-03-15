namespace Markwardt;

public interface IObservableCache<TKey, T> : IObservableReadOnlyCache<TKey, T>, IObservableList<T>
    where TKey : notnull
    where T : notnull
{
    void RemoveKey(IEnumerable<TKey> keys);
}

public static class ObservableCacheExtensions
{
    public static void RemoveKey<TKey, T>(this IObservableCache<TKey, T> cache, params TKey[] keys)
        where TKey : notnull
        where T : notnull
        => cache.RemoveKey(keys);

    public static void RemoveKey<TKey, T>(this IObservableCache<TKey, T> cache, TKey key)
        where TKey : notnull
        where T : notnull
        => cache.RemoveKey([key]);
}

public class ObservableCache<TKey, T> : ObservableList<T>, IObservableCache<TKey, T>
    where TKey : notnull
    where T : notnull
{
    [SuppressMessage("Sonar Code Quality", "S3427")]
    public ObservableCache(Func<T, TKey> keySelector, IEnumerable<T>? items = null)
        : base(items)
    {
        this.keySelector = keySelector;

        this.OutputToDictionary(dictionary, GetKey).DisposeWith(this);
    }

    private readonly Func<T, TKey> keySelector;
    private readonly Dictionary<TKey, T> dictionary = [];

    public IEnumerable<KeyValuePair<TKey, T>> Pairs => this.Select(x => new KeyValuePair<TKey, T>(GetKey(x), x));
    public IEnumerable<TKey> Keys => this.Select(GetKey);

    public TKey GetKey(T value)
        => keySelector(value);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        => dictionary.TryGetValue(key, out value);

    public void RemoveKey(IEnumerable<TKey> keys)
        => Remove(keys.Select(x => this.GetValue(x)).Where(x => x.HasValue).Select(x => x.Value));
}