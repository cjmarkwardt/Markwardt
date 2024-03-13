namespace Markwardt;

public interface IObservableReadOnlyCollection<T> : IObservableItems<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, IComplexDisposable
    where T : notnull;