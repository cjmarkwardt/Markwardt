namespace Markwardt;

public interface IObservableInsertionQueue<T> : IObservableReadOnlyList<T>, IInsertionQueue<T>
    where T : notnull
{
    new int Count { get; }
}