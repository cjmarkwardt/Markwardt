namespace Markwardt;

public interface IObservableReadOnlyDictionary<TKey, T> : IObservableReadOnlyList<KeyValuePair<TKey, T>>, IReadOnlyDictionary<TKey, T>
    where TKey : notnull
    where T : notnull
{
    new int Count { get; }
}

public static class ObservableReadOnlyDictionaryExtensions
{
    public static Maybe<T> GetValue<TKey, T>(this IObservableReadOnlyDictionary<TKey, T> list, TKey key)
        where TKey : notnull
        where T : notnull
        => list.TryGetValue(key, out T? value) ? value.Maybe() : default;

    public static IObservableReadOnlyDictionary<TKey, T> ObserveAsDictionary<TKey, T>(this IObservable<IChangeSet<KeyValuePair<TKey, T>>> changes)
        where TKey : notnull
        where T : notnull
        => new ObservableReadOnlyDictionary<TKey, T>(changes.AsObservableList());
}

public class ObservableReadOnlyDictionary<TKey, T> : ObservableReadOnlyList<KeyValuePair<TKey, T>>, IObservableReadOnlyDictionary<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableReadOnlyDictionary(DynamicData.IObservableList<KeyValuePair<TKey, T>> source)
        : base(source)
    {
        Connect().OnItemAdded(x => lookup.Add(x.Key, x.Value)).Subscribe().DisposeWith(this);
        Connect().OnItemRemoved(x => lookup.Remove(x.Key)).Subscribe().DisposeWith(this);
    }

    private readonly Dictionary<TKey, T> lookup = [];

    public T this[TKey key] => lookup[key];
    public IEnumerable<TKey> Keys => this.Select(x => x.Key);
    public IEnumerable<T> Values => this.Select(x => x.Value);

    public bool ContainsKey(TKey key)
        => lookup.ContainsKey(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        => lookup.TryGetValue(key, out value);
}