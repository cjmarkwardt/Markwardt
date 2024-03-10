using DynamicData;

namespace Markwardt;

public interface ICollectionStream<out T> : IObservable<ICollectionChange<T>>
    where T : notnull
{
    ICollectionStream<TSelected> Select<TSelected>(Func<T, TSelected> selector)
        where TSelected : notnull;

    ICollectionStream<TSelected> SelectMany<TSelected>(Func<T, IEnumerable<TSelected>> selector)
        where TSelected : notnull;

    ICollectionStream<T> Where(Func<T, bool> filter);

    ICollectionStream<T> Sort(IComparer<T> comparer);

    IKeyedCollectionStream<TKey, T> WithKeys<TKey>(Func<T, TKey> keySelector)
        where TKey : notnull;

    IObservableReadOnlyCollection<T> ToCollection();

    IObservableReadOnlyList<T> ToList();
}

public interface ICollectionStream<TKey, out T> : IObservable<ICollectionChange<T>>
    where TKey : notnull
    where T : notnull
{
    ICollectionStream<TKey, TSelected> Select<TSelected>(Func<T, TSelected> selector, Func<TSelected, TKey> keySelector)
        where TSelected : notnull;

    ICollectionStream<TKey, TSelected> SelectMany<TSelected>(Func<T, IEnumerable<TSelected>> selector, Func<TSelected, TKey> keySelector)
        where TSelected : notnull;

    ICollectionStream<TKey, T> Where(Func<T, bool> filter);

    ICollectionStream<TKey, T> Sort(IComparer<T> comparer);

    ICollectionStream<TSelected, T> ChangeKeys<TSelected>(Func<T, TSelected> keySelector)
        where TSelected : notnull;

    ICollectionStream<TSelected, T> SelectKeys<TSelected>(Func<TKey, TSelected> keySelector)
        where TSelected : notnull;

    ICollectionStream<TKey, T> WhereKeys(Func<TKey, bool> keyFilter);

    ICollectionStream<T> WithoutKeys();
}

public class CollectionStream<T>(IObservable<IChangeSet<T>> source) : ObservableItemsStream<T>(source), ICollectionStream<T>
    where T : notnull
{
    public ICollectionStream<TSelected> Select<TSelected>(Func<T, TSelected> selector)
        where TSelected : notnull
        => new CollectionStream<TSelected>(Source.Transform(selector));

    public ICollectionStream<TSelected> SelectMany<TSelected>(Func<T, IEnumerable<TSelected>> selector)
        where TSelected : notnull
        => new CollectionStream<TSelected>(Source.TransformMany(selector));

    public ICollectionStream<T> Where(Func<T, bool> filter)
        => new CollectionStream<T>(Source.Filter(filter));

    public ICollectionStream<T> Sort(IComparer<T> comparer)
        => new CollectionStream<T>(Source.Sort(comparer));

    public IKeyedCollectionStream<TKey, T> WithKeys<TKey>(Func<T, TKey> keySelector)
        where TKey : notnull
        => new KeyedCollectionStream<TKey, T>(Source, keySelector);

    public IObservableReadOnlyCollection<T> ToCollection()
        => ToList();

    public IObservableReadOnlyList<T> ToList()
        => new ObservableReadOnlyList<T>(Source.AsObservableList());
}