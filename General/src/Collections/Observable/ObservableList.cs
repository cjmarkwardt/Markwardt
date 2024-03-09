using DynamicData;

namespace Markwardt;

public interface IObservableList<T> : IObservableReadOnlyList<T>, IManyList<T>
    where T : notnull;

[SuppressMessage("Sonar Code Quality", "S3881")]
public class ObservableList<T> : ObservableReadOnlyList<T>, IObservableList<T>, IList
    where T : notnull
{
    public ObservableList(IEnumerable<T>? items = null)
        : base(new SourceList<T>())
    {
        if (items is not null)
        {
            Add(items);
        }
    }

    protected new ISourceList<T> Source => (ISourceList<T>)base.Source;

    public new T this[int index] { get => base[index]; set => Source.Edit(x => x[index] = value); }
    
    [SuppressMessage("Sonar Code Quality", "S1144")]
    bool ICollection<T>.IsReadOnly => false;

    [SuppressMessage("Sonar Code Quality", "S1144")]
    object? IList.this[int index] { get => this[index]; set => this[index] = (T)value!; }

    public void Add(IEnumerable<T> items)
        => Source.Edit(x => x.AddRange(items));

    public void Add(T item)
        => Source.Edit(x => x.Add(item));

    public void Insert(int index, T item)
        => Source.Edit(x => x.Insert(index, item));

    public void Insert(int index, IEnumerable<T> items)
        => Source.Edit(x => x.InsertRange(items, index));

    public void Remove(IEnumerable<T> items)
        => Source.Edit(x => x.RemoveMany(items));

    public bool Remove(T item)
    {
        bool result = default;
        Source.Edit(x => result = x.Remove(item));
        return result;
    }

    public void RemoveAt(int index)
        => Source.Edit(x => x.RemoveAt(index));

    public void RemoveAt(Range range)
    {
        (int offset, int length) = range.GetOffsetAndLength(Count);
        Source.Edit(x => x.RemoveRange(offset, length));
    }

    public void Clear()
        => Source.Edit(x => x.Clear());

    public bool Contains(T item)
        => Source.Items.Contains(item);

    public int IndexOf(T item)
        => Source.Items.IndexOf(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (T item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    int IList.Add(object? value)
    {
        int index = Count;
        Add((T)value!);
        return index;
    }

    void IList.Insert(int index, object? value)
        => Insert(index, (T)value!);

    void IList.Remove(object? value)
        => Remove((T)value!);
}
