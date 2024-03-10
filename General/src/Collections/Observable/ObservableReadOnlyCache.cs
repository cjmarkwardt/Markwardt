using DynamicData;

namespace Markwardt;

public interface IObservableReadOnlyCache<TKey, out T> : IObservableReadOnlyList<T>, IReadOnlyCache<TKey, T>
    where TKey : notnull
    where T : notnull
{
    new IKeyedCollectionStream<TKey, T> ObserveItems();
}

public class ObservableReadOnlyCache<TKey, T> : ObservableReadOnlyList<T>, IObservableReadOnlyCache<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableReadOnlyCache(DynamicData.IObservableList<T> source, Func<T, TKey> keySelector)
        : base(source)
    {
        KeySelector = keySelector;
        
        ObserveItems().AsAdds().Subscribe(x => Dictionary.Add(KeySelector(x), x)).DisposeWith(this);
        ObserveItems().AsRemoves().Subscribe(x => Dictionary.Remove(KeySelector(x))).DisposeWith(this);
    }

    protected Func<T, TKey> KeySelector { get; }
    protected IDictionary<TKey, T> Dictionary { get; } = new Dictionary<TKey, T>();

    public IMaybe<T> KeyedGet(TKey key)
        => Dictionary.TryGetValue(key, out T? value) ? value.AsMaybe() : Maybe<T>.Empty();

    public new IKeyedCollectionStream<TKey, T> ObserveItems()
        => new KeyedCollectionStream<TKey, T>(Source.Connect(), KeySelector);
}