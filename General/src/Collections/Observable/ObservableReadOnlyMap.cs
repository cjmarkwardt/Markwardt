using DynamicData;

namespace Markwardt;

public interface IObservableReadOnlyMap<TKey, out T> : IObservableReadOnlyList<IPair<TKey, T>>, IReadOnlyMap<TKey, T>
    where TKey : notnull
    where T : notnull
{
    new IPairCollectionStream<TKey, T> ObserveItems();
}

public class ObservableReadOnlyMap<TKey, T> : ObservableReadOnlyList<IPair<TKey, T>>, IObservableReadOnlyMap<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableReadOnlyMap(DynamicData.IObservableList<IPair<TKey, T>> source)
        : base(source)
    {
        ObserveItems().AsAdds().Subscribe(x => Dictionary.Add(x.Key, x.Value)).DisposeWith(this);
        ObserveItems().AsRemoves().Subscribe(x => Dictionary.Remove(x.Key)).DisposeWith(this);
    }

    protected IDictionary<TKey, T> Dictionary { get; } = new Dictionary<TKey, T>();

    public IMaybe<T> KeyedGet(TKey key)
        => Dictionary.TryGetValue(key, out T? value) ? value.AsMaybe() : Maybe<T>.Empty();

    public new IPairCollectionStream<TKey, T> ObserveItems()
        => new PairCollectionStream<TKey, T>(Source.Connect());
}