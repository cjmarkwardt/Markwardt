namespace Markwardt;

public interface IObservableReadOnlyList<T> : DynamicData.IObservableList<T>, IReadOnlyList<T>, IExtendedDisposable
    where T : notnull
{
    new int Count { get; }
}

public static class ObservableReadOnlyListExtensions
{
    public static DynamicData.IObservableList<T> AsObservableList<T>(this IObservableReadOnlyList<T> list)
        where T : notnull
        => list;

    public static IObservableReadOnlyList<T> ObserveAsList<T>(this IObservable<IChangeSet<T>> changes)
        where T : notnull
        => new ObservableReadOnlyList<T>(changes.AsObservableList());
}

public class ObservableReadOnlyList<T> : ExtendedDisposable, IObservableReadOnlyList<T>, IList, INotifyCollectionChanged
    where T : notnull
{
    public ObservableReadOnlyList(DynamicData.IObservableList<T> source)
    {
        this.source = source.DisposeWith(this);
        notifier = new CollectionNotifier<T>(source.Connect()).DisposeWith(this);
        source.CountChanged.Subscribe(_ => this.RaisePropertyChanged(nameof(Count))).DisposeWith(this);
    }

    event NotifyCollectionChangedEventHandler? INotifyCollectionChanged.CollectionChanged { add => notifier.CollectionChanged += value; remove => notifier.CollectionChanged -= value; }

    private readonly DynamicData.IObservableList<T> source;
    private readonly ICollectionNotifier notifier;

    public T this[int index] => source.Items.ElementAt(0);
    public int Count => source.Count;
    public IObservable<int> CountChanged => source.CountChanged;
    public IEnumerable<T> Items => source.Items;

    bool IList.IsFixedSize => false;
    bool IList.IsReadOnly => true;
    bool ICollection.IsSynchronized => false;
    object ICollection.SyncRoot => this;
    object? IList.this[int index] { get => this[index]; set => throw new NotSupportedException(); }

    public IObservable<IChangeSet<T>> Connect(Func<T, bool>? predicate = null)
        => source.Connect(predicate);

    public IObservable<IChangeSet<T>> Preview(Func<T, bool>? predicate = null)
        => source.Preview(predicate);

    public IEnumerator<T> GetEnumerator()
        => source.Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    bool IList.Contains(object? value)
        => value is T casted && this.Contains(casted);

    int IList.IndexOf(object? value)
        => value is T casted ? this.IndexOf(casted) : -1;

    void ICollection.CopyTo(Array array, int index)
    {
        foreach (T item in this)
        {
            array.SetValue(item, index++);
        }
    }

    int IList.Add(object? value)
        => throw new NotSupportedException();

    void IList.Clear()
        => throw new NotSupportedException();

    void IList.Insert(int index, object? value)
        => throw new NotSupportedException();

    void IList.Remove(object? value)
        => throw new NotSupportedException();

    void IList.RemoveAt(int index)
        => throw new NotSupportedException();
}