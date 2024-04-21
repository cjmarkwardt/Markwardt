namespace Markwardt;

public interface IObservableReadOnlyLookupList<TKey, T> : IObservableReadOnlyList<T>
    where TKey : notnull
    where T : notnull
{
    new int Count { get; }
    IReadOnlyDictionary<TKey, T> Lookup { get; }
}

public static class ObservableReadOnlyLookupListExtensions
{
    public static bool ContainsKey<TKey, T>(this IObservableReadOnlyLookupList<TKey, T> list, TKey key)
        where TKey : notnull
        where T : notnull
        => list.Lookup.ContainsKey(key);

    public static bool TryGetValue<TKey, T>(this IObservableReadOnlyLookupList<TKey, T> list, TKey key, [MaybeNullWhen(false)] out T value)
        where TKey : notnull
        where T : notnull
        => list.TryGetValue(key, out value);

    public static Maybe<T> GetValue<TKey, T>(this IObservableReadOnlyLookupList<TKey, T> list, TKey key)
        where TKey : notnull
        where T : notnull
        => list.TryGetValue(key, out T? value) ? value.Maybe() : default;

    public static IObservableReadOnlyLookupList<TKey, T> ObserveAsLookupList<TKey, T>(this IObservable<IChangeSet<T>> changes, Func<T, TKey> keySelector)
        where TKey : notnull
        where T : notnull
        => new ObservableReadOnlyLookupList<TKey, T>(changes.AsObservableList(), keySelector);
}

public class ObservableReadOnlyLookupList<TKey, T> : ObservableReadOnlyList<T>, IObservableReadOnlyLookupList<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableReadOnlyLookupList(DynamicData.IObservableList<T> source, Func<T, TKey> keySelector)
        : base(source)
    {
        Connect().OnItemAdded(x => lookup.Add(keySelector(x), x)).Subscribe().DisposeWith(this);
        Connect().OnItemRemoved(x => lookup.Remove(keySelector(x))).Subscribe().DisposeWith(this);
    }

    private readonly Dictionary<TKey, T> lookup = [];

    public IReadOnlyDictionary<TKey, T> Lookup => lookup;
}