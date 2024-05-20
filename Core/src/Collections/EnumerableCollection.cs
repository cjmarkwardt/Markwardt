namespace Markwardt;

public static class EnumerableCollectionExtensions
{
    public static ICollection<T> AsCollection<T>(this IEnumerable<T> enumerable)
        => new EnumerableCollection<T>(enumerable);
}

public class EnumerableCollection<T>(IEnumerable<T> source) : ICollection<T>
{
    public int Count => source.Count();

    public bool IsReadOnly => true;

    public void Add(T item) => throw new NotSupportedException();

    public void Clear() => throw new NotSupportedException();

    public bool Contains(T item) => source.Contains(item);

    public void CopyTo(T[] array, int arrayIndex)
    {
        foreach (T item in this)
        {
            array[arrayIndex++] = item;
        }
    }

    public IEnumerator<T> GetEnumerator() => source.GetEnumerator();

    public bool Remove(T item) => throw new NotSupportedException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}