namespace Markwardt;

public interface IManyList<T> : IManyCollection<T>, IList<T>
{
    void Insert(int index, IEnumerable<T> items);
    void RemoveAt(Range range);
    void ReplaceAll(IEnumerable<T> items);
}

public static class ManyListExtensions
{
    public static void Insert<T>(this IManyList<T> list, params T[] items)
        => list.Insert(items);

    public static IList<T> Get<T>(this IManyList<T> list, Range range)
        => range.Iterate(list).ToList();
}