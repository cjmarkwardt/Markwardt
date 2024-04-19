namespace Markwardt;

public interface IConsumptionQueue<out T> : IReadOnlyList<T>
{
    IEnumerable<T> Dequeue(int count);
}

public static class ConsumptionQueueExtensions
{
    public static IEnumerable<T> DequeueAll<T>(this IConsumptionQueue<T> queue)
        => queue.Dequeue(queue.Count);

    public static Maybe<T> Dequeue<T>(this IConsumptionQueue<T> queue)
        => queue.Count > 0 ? queue.Dequeue(1).First().Maybe() : default;
}