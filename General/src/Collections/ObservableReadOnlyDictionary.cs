namespace Markwardt;

public interface IObservableReadOnlyDictionary<TKey, T> : IObservableReadOnlyList<KeyValuePair<TKey, T>>, IReadOnlyDictionary<TKey, T>
    where TKey : notnull
    where T : notnull;

public class ObservableReadOnlyDictionary<TKey, T> : ObservableReadOnlyList<KeyValuePair<TKey, T>>, IObservableReadOnlyDictionary<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableReadOnlyDictionary(DynamicData.IObservableList<KeyValuePair<TKey, T>> source)
        : base(source)
    {
        this.OutputToDictionary(dictionary, x => x.Key, x => x.Value).DisposeWith(this);
    }

    private readonly Dictionary<TKey, T> dictionary = [];

    public T this[TKey key] => dictionary[key];

    public IEnumerable<TKey> Keys => this.Select(x => x.Key);
    public IEnumerable<T> Values => this.Select(x => x.Value);

    public bool ContainsKey(TKey key)
        => dictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        => dictionary.TryGetValue(key, out value);
}