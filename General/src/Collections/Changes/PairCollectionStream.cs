using DynamicData;

namespace Markwardt;

public interface IPairCollectionStream<TKey, out T> : ICollectionStream<IPair<TKey, T>>
    where TKey : notnull
    where T : notnull
{
    ICollectionStream<T> WithoutKeys();

    IObservableReadOnlyMap<TKey, T> ToMap();
}

public class PairCollectionStream<TKey, T>(IObservable<IChangeSet<IPair<TKey, T>>> source) : CollectionStream<IPair<TKey, T>>(source), IPairCollectionStream<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ICollectionStream<T> WithoutKeys()
        => new CollectionStream<T>(Source.Transform(x => x.Value));

    public IObservableReadOnlyMap<TKey, T> ToMap()
        => new ObservableReadOnlyMap<TKey, T>(Source);
}