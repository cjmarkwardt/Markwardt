namespace Markwardt;

public interface IObservableReadOnlyCache<TKey, T> : IObservableReadOnlyList<T>
    where TKey : notnull
    where T : notnull
{
    IEnumerable<KeyValuePair<TKey, T>> Pairs { get; }
    IEnumerable<TKey> Keys { get; }
    
    TKey GetKey(T value);
    bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value);
}

public static class ObservableReadOnlyCacheExtensions
{
    public static Maybe<T> GetValue<TKey, T>(this IObservableReadOnlyCache<TKey, T> cache, TKey key)
        where TKey : notnull
        where T : notnull
        => cache.TryGetValue(key, out T? value) ? value.Maybe() : default;

    public static bool ContainsKey<TKey, T>(this IObservableReadOnlyCache<TKey, T> cache, TKey key)
        where TKey : notnull
        where T : notnull
        => cache.TryGetValue(key, out _);
}

public class ObservableReadOnlyCache<TKey, T> : ObservableReadOnlyList<T>, IObservableReadOnlyCache<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableReadOnlyCache(DynamicData.IObservableList<T> source, Func<T, TKey> keySelector)
        : base(source)
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
}