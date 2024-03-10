namespace Markwardt;

public interface IObservableMap<TKey, T> : IObservableReadOnlyMap<TKey, T>, IObservableList<IPair<TKey, T>>, IMap<TKey, T>
    where TKey : notnull
    where T : notnull;

public class ObservableMap<TKey, T> : ObservableList<IPair<TKey, T>>, IObservableMap<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableMap(ItemDisposal itemDisposal, IEnumerable<IPair<TKey, T>>? items = null)
        : base(itemDisposal, items)
    {
        ObserveItems().AsAdds().Subscribe(x => Dictionary.Add(x.Key, x.Value)).DisposeWith(this);
        ObserveItems().AsRemoves().Subscribe(x => Dictionary.Remove(x.Key)).DisposeWith(this);
    }

    public ObservableMap(IEnumerable<IPair<TKey, T>>? items = null)
        : this(ItemDisposal.None, items) { }

    protected IDictionary<TKey, T> Dictionary { get; } = new Dictionary<TKey, T>();

    public IMaybe<T> KeyedGet(TKey key)
        => Dictionary.TryGetValue(key, out T? value) ? value.AsMaybe() : Maybe<T>.Empty();

    public void KeyedRemove(IEnumerable<TKey> keys)
        => Remove(keys.Select(x => (Key: x, Value: KeyedGet(x))).Where(x => x.Value.HasValue).Select(x => x.Value.Value.AsPair(x.Key)));

    public new IPairCollectionStream<TKey, T> ObserveItems()
        => new PairCollectionStream<TKey, T>(Source.Connect());
}