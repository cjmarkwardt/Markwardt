namespace Markwardt;

public interface IObservableConsumptionQueue<T> : IObservableReadOnlyList<T>, IConsumptionQueue<T>
    where T : notnull
{
    new int Count { get; }
}

public static class ObservableConsumptionQueueExtensions
{
    public static IObservable<T> Consume<T>(this IObservableConsumptionQueue<T> queue)
        where T : notnull
        => queue.Connect().SelectMany(_ => queue.DequeueAll()).StartWith(queue.DequeueAll());
}