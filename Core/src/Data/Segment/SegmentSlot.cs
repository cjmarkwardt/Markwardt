namespace Markwardt;

public static class SegmentSlot
{
    public static object Create(Type type, IDataSegmentTyper segmentTyper, IDataHandler handler, DataDictionary dictionary, string key)
        => typeof(SegmentSlot<>).MakeGenericType(type).GetConstructor([typeof(IDataSegmentTyper), typeof(IDataHandler), typeof(DataDictionary), typeof(string)]).NotNull().Invoke([segmentTyper, handler, dictionary, key]);
}

public interface ISegmentSlot<T>
    where T : class
{
    bool HasValue { get; }

    bool Is<TDerived>()
        where TDerived : class, T;

    TDerived Get<TDerived>()
        where TDerived : class, T;

    TDerived Set<TDerived>()
        where TDerived : class, T;

    void Clear();
}

public static class SegmentSlotExtensions
{
    public static T Get<T>(this ISegmentSlot<T> slot)
        where T : class
        => slot.Get<T>();

    public static T Set<T>(this ISegmentSlot<T> slot)
        where T : class
        => slot.Set<T>();
}

public class SegmentSlot<T>(IDataSegmentTyper segmentTyper, IDataHandler handler, DataDictionary dictionary, string key) : ISegmentSlot<T>
    where T : class
{
    private readonly DataSegmentReader<T> reader = new(segmentTyper, handler);

    public bool HasValue => dictionary.ContainsKey(key);

    public bool Is<TDerived>()
        where TDerived : class, T
        => dictionary.TryGetValue(key, out IDataNode? node) && segmentTyper.Is(node, typeof(TDerived));

    public TDerived Get<TDerived>()
        where TDerived : class, T
        => reader.Read<TDerived>(dictionary[key]);

    public TDerived Set<TDerived>()
        where TDerived : class, T
    {
        dictionary[key] = segmentTyper.Create(typeof(TDerived));
        return Get<TDerived>();
    }

    public void Clear()
        => dictionary.Remove(key);
}