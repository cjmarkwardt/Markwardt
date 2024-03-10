using DynamicData;

namespace Markwardt;

public interface IKeyedCollectionStream<TKey, out T> : ICollectionStream<TKey, T>
    where TKey : notnull
    where T : notnull
{
    IObservableReadOnlyCache<TKey, T> ToCache();
}

public class KeyedCollectionStream<TKey, T>(IObservable<IChangeSet<T>> source, Func<T, TKey> getKey) : ObservableItemsStream<T>(source), IKeyedCollectionStream<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ICollectionStream<TKey, TSelected> Select<TSelected>(Func<T, TSelected> selector, Func<TSelected, TKey> keySelector)
        where TSelected : notnull
        => new KeyedCollectionStream<TKey, TSelected>(Source.Transform(selector), keySelector);

    public ICollectionStream<TKey, TSelected> SelectMany<TSelected>(Func<T, IEnumerable<TSelected>> selector, Func<TSelected, TKey> keySelector)
        where TSelected : notnull
        => new KeyedCollectionStream<TKey, TSelected>(Source.TransformMany(selector), keySelector);

    public ICollectionStream<TKey, T> Where(Func<T, bool> filter)
        => new KeyedCollectionStream<TKey, T>(Source.Filter(filter), getKey);

    public ICollectionStream<TKey, T> Sort(IComparer<T> comparer)
        => new KeyedCollectionStream<TKey, T>(Source.Sort(comparer), getKey);

    public ICollectionStream<TSelected, T> ChangeKeys<TSelected>(Func<T, TSelected> keySelector)
        where TSelected : notnull
        => new KeyedCollectionStream<TSelected, T>(Source, keySelector);

    public ICollectionStream<TSelected, T> SelectKeys<TSelected>(Func<TKey, TSelected> keySelector)
        where TSelected : notnull
        => new KeyedCollectionStream<TSelected, T>(Source, x => keySelector(getKey(x)));

    public ICollectionStream<TKey, T> WhereKeys(Func<TKey, bool> keyFilter)
        => new KeyedCollectionStream<TKey, T>(Source.Filter(x => keyFilter(getKey(x))), getKey);

    public ICollectionStream<T> WithoutKeys()
        => new CollectionStream<T>(Source);

    public IObservableReadOnlyCache<TKey, T> ToCache()
        => new ObservableReadOnlyCache<TKey, T>(Source.AsObservableList(), getKey);
}