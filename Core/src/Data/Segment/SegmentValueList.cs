namespace Markwardt;

public static class SegmentValueList
{
    public static object Create(Type type, IDataHandler handler, DataList list)
        => typeof(SegmentValueList<>).MakeGenericType(type).GetConstructor([typeof(IDataHandler), typeof(DataList)]).NotNull().Invoke([handler, list]);
}

public class SegmentValueList<T>(IDataHandler handler, DataList data) : IList<T>
    where T : notnull
{
    private readonly DataReader<T> reader = new(handler);

    public T this[int index] { get => reader.Read(data[index]); set => data[index] = reader.Write(value); }

    public int Count => data.Count;

    public bool IsReadOnly => false;

    public void Add(T item)
        => data.Add(reader.Write(item));

    public void Clear()
        => data.Clear();

    public bool Contains(T item)
        => data.Contains(reader.Write(item));

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (T item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public IEnumerator<T> GetEnumerator()
        => data.Select(reader.Read).GetEnumerator();

    public int IndexOf(T item)
        => data.IndexOf(reader.Write(item));

    public void Insert(int index, T item)
        => data.Insert(index, reader.Write(item));

    public bool Remove(T item)
        => data.Remove(reader.Write(item));

    public void RemoveAt(int index)
        => data.RemoveAt(index);

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}