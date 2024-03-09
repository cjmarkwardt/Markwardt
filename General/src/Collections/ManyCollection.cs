namespace Markwardt;

public interface IManyCollection<T> : ICollection<T>
{
    void Add(IEnumerable<T> items);
    void Remove(IEnumerable<T> items);
}

public static class ManyCollectionExtensions
{
    public static void Add<T>(this IManyCollection<T> collection, params T[] items)
        => collection.Add(items);

    public static void Remove<T>(this IManyCollection<T> collection, params T[] items)
        => collection.Remove(items);
}