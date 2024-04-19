namespace Markwardt;

public interface IInsertionQueue<T> : IReadOnlyList<T>
{
    void Enqueue(IEnumerable<T> items);
}

public static class InsertionQueueExtensions
{
    public static void Enqueue<T>(this IInsertionQueue<T> queue, T item)
        => queue.Enqueue([item]);
}