namespace Markwardt;

public interface IObservableCollection<T> : IObservableReadOnlyCollection<T>, IManyCollection<T>
    where T : notnull;