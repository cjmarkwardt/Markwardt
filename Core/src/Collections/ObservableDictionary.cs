namespace Markwardt;

public interface IObservableDictionary<TKey, T> : IObservableReadOnlyDictionary<TKey, T>, IObservableList<KeyValuePair<TKey, T>>, IDictionary<TKey, T>
    where TKey : notnull
    where T : notnull
{
    new int Count { get; }
}

public class ObservableDictionary<TKey, T> : ObservableList<KeyValuePair<TKey, T>>, IObservableDictionary<TKey, T>
    where TKey : notnull
    where T : notnull
{
    public ObservableDictionary(ISourceList<KeyValuePair<TKey, T>> source)
        : base(source)
    {
        keys = new EnumerableCollection<TKey>(Keys);
        values = new EnumerableCollection<T>(Values);

        Connect().OnItemRemoved(OnRemove).Subscribe().DisposeWith(this);
        Connect().OnItemAdded(x => lookup.Add(x.Key, x.Value)).Subscribe().DisposeWith(this);
        Connect().OnItemRemoved(x => lookup.Remove(x.Key)).Subscribe().DisposeWith(this);
    }

    public ObservableDictionary()
        : this(new SourceList<KeyValuePair<TKey, T>>()) { }

    public ObservableDictionary(IEnumerable<KeyValuePair<TKey, T>> items)
        : this()
        => Add(items);

    private readonly Dictionary<TKey, T> lookup = [];
    private readonly ICollection<TKey> keys;
    private readonly ICollection<T> values;

    public IEnumerable<TKey> Keys => this.Select(x => x.Key);
    public IEnumerable<T> Values => this.Select(x => x.Value);

    ICollection<TKey> IDictionary<TKey, T>.Keys => keys;
    ICollection<T> IDictionary<TKey, T>.Values => values;

    public T this[TKey key]
    {
        get => lookup[key];
        set
        {
            Remove(key);
            Add(key, value);
        }
    }

    public bool ContainsKey(TKey key)
        => lookup.ContainsKey(key);

    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        => lookup.TryGetValue(key, out value);

    public void Add(TKey key, T value)
        => Add(new KeyValuePair<TKey, T>(key, value));

    public bool Remove(TKey key)
    {
        if (TryGetValue(key, out T? value))
        {
            Remove(new KeyValuePair<TKey, T>(key, value));
            return true;
        }

        return false;
    }

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        if (ItemDisposal is ItemDisposal.OnDisposal || ItemDisposal is ItemDisposal.Full)
        {
            this.ForEach(x => x.Value.DisposeWith(this));
        }
    }

    private void OnRemove(KeyValuePair<TKey, T> pair)
    {
        if (ItemDisposal is ItemDisposal.OnRemoval || ItemDisposal is ItemDisposal.Full)
        {
            this.DisposeInBackground(pair.Value);
        }
    }

    private sealed class EnumerableCollection<TItem>(IEnumerable<TItem> items) : ICollection<TItem>
    {
        public int Count => items.Count();

        public bool IsReadOnly => true;

        public void Add(TItem item)
            => throw new NotSupportedException();

        public void Clear()
            => throw new NotSupportedException();

        public bool Remove(TItem item)
            => throw new NotSupportedException();

        public bool Contains(TItem item)
            => items.Contains(item);

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            foreach (TItem item in items)
            {
                array[arrayIndex++] = item;
            }
        }

        public IEnumerator<TItem> GetEnumerator()
            => items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}