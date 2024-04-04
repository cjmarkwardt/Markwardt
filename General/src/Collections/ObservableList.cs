namespace Markwardt;

public interface IObservableList<T> : IObservableReadOnlyList<T>, ISourceList<T>, IManyList<T>
    where T : notnull
{
    new int Count { get; }
}

public class ObservableList<T> : ObservableReadOnlyList<T>, IObservableList<T>, IList
    where T : notnull
{
    public ObservableList(ISourceList<T> source)
        : base(source)
    {
        this.source = source;
        
        Connect().OnItemRemoved(OnRemove).Subscribe().DisposeWith(this);
    }

    public ObservableList()
        : this(new SourceList<T>()) { }

    public ObservableList(IEnumerable<T> items)
        : this()
        => Add(items);

    private readonly ISourceList<T> source;
    
    public ItemDisposal ItemDisposal { get; set; } = ItemDisposal.None;

    public new T this[int index] { get => base[index]; set => Edit(x => x[index] = value); }

    public bool IsReadOnly => false;

    object? IList.this[int index]
    {
        get => this[index];
        set
        {
            if (value is T casted)
            {
                this[index] = casted;
            }
        }
    }

    public void Add(IEnumerable<T> items)
        => Edit(x => x.AddRange(items));

    public void Add(T item)
        => Edit(x => x.Add(item));

    public void Clear()
        => Edit(x => x.Clear());

    public bool Contains(T item)
        => source.Items.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (T item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public void Edit(Action<IExtendedList<T>> updateAction)
        => source.Edit(updateAction);

    public int IndexOf(T item)
        => source.Items.IndexOf(item);

    public void Insert(int index, IEnumerable<T> items)
        => Edit(x => x.InsertRange(items, index));

    public void Insert(int index, T item)
        => Edit(x => x.Insert(index, item));

    public void Remove(IEnumerable<T> items)
        => Edit(x => x.RemoveMany(items));

    public bool Remove(T item)
    {
        bool result = false;
        Edit(x => result = x.Remove(item));
        return result;
    }

    public void RemoveAt(Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(Count);
        Edit(x => x.RemoveRange(offset, length));
    }

    public void RemoveAt(int index)
        => Edit(x => x.RemoveAt(index));

    int IList.Add(object? value)
    {
        if (value is T casted)
        {
            Add(casted);
            return Count - 1;
        }

        return -1;
    }

    void IList.Clear()
        => Clear();

    void IList.Insert(int index, object? value)
    {
        if (value is T casted)
        {
            Insert(index, casted);
        }
    }

    void IList.Remove(object? value)
    {
        if (value is T casted)
        {
            Remove(casted);
        }
    }

    void IList.RemoveAt(int index)
        => RemoveAt(index);

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        if (ItemDisposal is ItemDisposal.OnDisposal || ItemDisposal is ItemDisposal.Full)
        {
            this.ForEach(x => x.DisposeWith(this));
        }
    }

    private void OnRemove(T item)
    {
        if (ItemDisposal is ItemDisposal.OnRemoval || ItemDisposal is ItemDisposal.Full)
        {
            this.DisposeInBackground(item);
        }
    }
}