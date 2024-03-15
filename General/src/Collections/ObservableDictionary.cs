namespace Markwardt;

public interface IObservableDictionary<TKey, T> : IObservableReadOnlyDictionary<TKey, T>, IObservableList<KeyValuePair<TKey, T>>, IDictionary<TKey, T>
    where TKey : notnull
    where T : notnull;

public class ObservableDictionary<TKey, T> : ObservableList<KeyValuePair<TKey, T>>, IObservableDictionary<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, T>>? items = null)
        : base(items)
    {
        keys = new(Keys);
        values = new(Values);

        this.OutputToDictionary(dictionary, x => x.Key, x => x.Value).DisposeWith(this);
    }

    private readonly Dictionary<TKey, T> dictionary = [];
    private readonly ViewCollection<TKey> keys;
    private readonly ViewCollection<T> values;

    public IEnumerable<TKey> Keys => this.Select(x => x.Key);
    public IEnumerable<T> Values => this.Select(x => x.Value);

    ICollection<TKey> IDictionary<TKey, T>.Keys => keys;
    ICollection<T> IDictionary<TKey, T>.Values => values;

    public T this[TKey key]
    {
        get => dictionary[key];
        set
        {
            Remove(key);
            Add(key, value);
        }
    }

    public bool ContainsKey(TKey key)
        => dictionary.ContainsKey(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        => dictionary.TryGetValue(key, out value);

    public void Add(TKey key, T value)
        => Add(new KeyValuePair<TKey, T>(key, value));

    public bool Remove(TKey key)
    {
        if (TryGetValue(key, out T? value))
        {
            return Remove(new KeyValuePair<TKey, T>(key, value));
        }

        return false;
    }

    private sealed class ViewCollection<TView>(IEnumerable<TView> target) : ICollection<TView>
    {
        public int Count => target.Count();
        public bool IsReadOnly => true;

        public void Add(TView item)
            => throw new NotSupportedException();

        public void Clear()
            => throw new NotSupportedException();

        public bool Remove(TView item)
            => throw new NotSupportedException();

        public bool Contains(TView item)
            => target.Contains(item);

        public void CopyTo(TView[] array, int arrayIndex)
        {
            foreach (TView item in target)
            {
                array[arrayIndex++] = item;
            }
        }

        public IEnumerator<TView> GetEnumerator()
            => target.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}