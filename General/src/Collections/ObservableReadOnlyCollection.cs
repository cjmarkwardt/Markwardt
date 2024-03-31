namespace Markwardt;

public interface IObservableReadOnlyCollection<T> : IObservableItems<T>, IReadOnlyCollection<T>, INotifyCollectionChanged, IExtendedDisposable
    where T : notnull;