namespace Markwardt;

public interface IObservableReadOnlyList<T> : IObservableReadOnlyCollection<T>, IReadOnlyList<T>
    where T : notnull;

public class ObservableReadOnlyList<T> : ExtendedDisposable, IObservableReadOnlyList<T>, IList
    where T : notnull
{
    public ObservableReadOnlyList(DynamicData.IObservableList<T> source)
    {
        Source = source.DisposeWith(this);
        Source.CountChanged.Subscribe(() => this.RaisePropertyChanged(nameof(Count))).DisposeWith(this);
        Source.Connect().ToNotifier().DisposeWith(this).CollectionChanged += (sender, arguments) => CollectionChanged?.Invoke(sender, arguments);
    }

    protected DynamicData.IObservableList<T> Source { get; }

    public T this[int index] => Source.Items.ElementAt(index);

    public int Count => Source.Count;

    [SuppressMessage("Sonar Code Quality", "S1144")]
    bool IList.IsFixedSize => false;

    [SuppressMessage("Sonar Code Quality", "S1144")]
    bool IList.IsReadOnly => false;

    [SuppressMessage("Sonar Code Quality", "S1144")]
    bool ICollection.IsSynchronized => false;

    [SuppressMessage("Sonar Code Quality", "S1144")]
    object ICollection.SyncRoot => this;

    [SuppressMessage("Sonar Code Quality", "S1144")]
    object? IList.this[int index] { get => this[index]; set => throw new NotSupportedException(); }

    [SuppressMessage("Sonar Code Quality", "S3264")]
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public IObservable<IChangeSet<T>> Observe()
        => Source.Connect();

    public IEnumerator<T> GetEnumerator()
        => Source.Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    [SuppressMessage("Sonar Code Quality", "S1172")]
    int IList.Add(object? value)
        => throw new NotSupportedException();

    [SuppressMessage("Sonar Code Quality", "S1172")]
    void IList.Insert(int index, object? value)
        => throw new NotSupportedException();

    [SuppressMessage("Sonar Code Quality", "S1172")]
    void IList.Remove(object? value)
        => throw new NotSupportedException();

    [SuppressMessage("Sonar Code Quality", "S1172")]
    void IList.RemoveAt(int index)
        => throw new NotSupportedException();

    void IList.Clear()
        => throw new NotSupportedException();

    bool IList.Contains(object? value)
        => this.Contains((T)value!);

    int IList.IndexOf(object? value)
        => this.IndexOf((T)value!);

    void ICollection.CopyTo(Array array, int index)
    {
        foreach (T item in this)
        {
            array.SetValue(item, index++);
        }
    }
}