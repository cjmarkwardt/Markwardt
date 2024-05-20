namespace Markwardt;

public static class SegmentList
{
    public static object Create(Type type, IDataSegmentTyper segmentTyper, IDataHandler handler, DataList list)
        => typeof(SegmentList<>).MakeGenericType(type).GetConstructor([typeof(IDataSegmentTyper), typeof(IDataHandler), typeof(DataList)]).NotNull().Invoke([segmentTyper, handler, list]);
}

public interface ISegmentList<T> : IEnumerable<T>
    where T : class
{
    int Count { get; }

    bool Is<TDerived>(int index)
        where TDerived : class, T;

    TDerived Get<TDerived>(int index)
        where TDerived : class, T;
    
    TDerived Add<TDerived>()
        where TDerived : class, T;
    
    TDerived Insert<TDerived>(int index)
        where TDerived : class, T;

    void RemoveAt(int index);

    void Clear();
}

public static class SegmentListExtensions
{
    public static T Get<T>(this ISegmentList<T> list, int index)
        where T : class
        => list.Get<T>(index);

    public static T Add<T>(this ISegmentList<T> list)
        where T : class
        => list.Add<T>();

    public static T Insert<T>(this ISegmentList<T> list, int index)
        where T : class
        => list.Insert<T>(index);
}

public class SegmentList<T>(IDataSegmentTyper segmentTyper, IDataHandler handler, DataList data) : ISegmentList<T>
    where T : class
{
    private readonly DataSegmentReader<T> reader = new(segmentTyper, handler);

    public int Count => data.Count;

    public bool Is<TDerived>(int index)
        where TDerived : class, T
        => segmentTyper.Is(data[index], typeof(TDerived));

    public TDerived Get<TDerived>(int index)
        where TDerived : class, T
        => reader.Read<TDerived>(data[index]);

    public TDerived Add<TDerived>()
        where TDerived : class, T
    {
        IDataNode segmentData = segmentTyper.Create(typeof(TDerived));
        data.Add(segmentData);
        return reader.Read<TDerived>(segmentData);
    }

    public TDerived Insert<TDerived>(int index)
        where TDerived : class, T
    {
        IDataNode segmentData = segmentTyper.Create(typeof(TDerived));
        data.Insert(index, segmentData);
        return reader.Read<TDerived>(segmentData);
    }

    public void RemoveAt(int index)
        => data.RemoveAt(index);

    public void Clear()
        => data.Clear();

    public IEnumerator<T> GetEnumerator()
        => data.Select(reader.Read<T>).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}
