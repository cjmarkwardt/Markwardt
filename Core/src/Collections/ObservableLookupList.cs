namespace Markwardt;

public interface IObservableLookupList<TKey, T> : IObservableReadOnlyLookupList<TKey, T>, IObservableList<T>
    where TKey : notnull
    where T : notnull
{
    new int Count { get; }
}

public static class ObservableLookupListExtensions
{
    public static bool Remove<TKey, T>(this IObservableLookupList<TKey, T> list, TKey key)
        where TKey : notnull
        where T : notnull
    {
        if (list.TryGetValue(key, out T? value))
        {
            list.Remove(value);
            return true;
        }

        return false;
    }
}

public class ObservableLookupList<TKey, T> : ObservableList<T>, IObservableLookupList<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableLookupList(ISourceList<T> source, Func<T, TKey> keySelector)
        : base(source)
    {
        Connect().OnItemAdded(x => lookup.Add(keySelector(x), x)).Subscribe().DisposeWith(this);
        Connect().OnItemRemoved(x => lookup.Remove(keySelector(x))).Subscribe().DisposeWith(this);
    }

    public ObservableLookupList(Func<T, TKey> keySelector)
        : this(new SourceList<T>(), keySelector) { }

    public ObservableLookupList(Func<T, TKey> keySelector, IEnumerable<T> items)
        : this(keySelector)
        => Add(items);

    private readonly Dictionary<TKey, T> lookup = [];

    public IReadOnlyDictionary<TKey, T> Lookup => lookup;
}