namespace Markwardt;

public interface IObservableReadOnlyCollection<out T> : IReadOnlyCollection<T>, INotifyCollectionChanged, IComplexDisposable
    where T : notnull
{
    ICollectionStream<T> ObserveItems();
}