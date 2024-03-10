namespace Markwardt;

public interface IObservableCache<TKey, T> : IObservableReadOnlyCache<TKey, T>, IObservableList<T>, ICache<TKey, T>
    where TKey : notnull
    where T : notnull;

public class ObservableCache<TKey, T> : ObservableList<T>, IObservableCache<TKey, T>
    where TKey : notnull
    where T : notnull
{
    [SuppressMessage("Sonar Code Quality", "S3427")]
    public ObservableCache(Func<T, TKey> keySelector, ItemDisposal itemDisposal, IEnumerable<T>? items = null)
        : base(itemDisposal, items)
    {
        KeySelector = keySelector;

        ObserveItems().AsAdds().Subscribe(x => Dictionary.Add(KeySelector(x), x)).DisposeWith(this);
        ObserveItems().AsRemoves().Subscribe(x => Dictionary.Remove(KeySelector(x))).DisposeWith(this);
    }

    public ObservableCache(Func<T, TKey> keySelector, IEnumerable<T>? items = null)
        : this(keySelector, ItemDisposal.None, items) { }

    protected Func<T, TKey> KeySelector { get; }
    protected IDictionary<TKey, T> Dictionary { get; } = new Dictionary<TKey, T>();

    public IMaybe<T> KeyedGet(TKey key)
        => Dictionary.TryGetValue(key, out T? value) ? value.AsMaybe() : Maybe<T>.Empty();

    public void KeyedRemove(IEnumerable<TKey> keys)
        => Remove(keys.Select(KeyedGet).Where(x => x.HasValue).Select(x => x.Value));

    public new IKeyedCollectionStream<TKey, T> ObserveItems()
        => new KeyedCollectionStream<TKey, T>(Source.Connect(), KeySelector);
}