namespace Markwardt;

public interface ICollectionNotifier : IExtendedDisposable, INotifyCollectionChanged;

public static class CollectionNotifierExtensions
{
    public static ICollectionNotifier ToNotifier<T>(this IObservable<IChangeSet<T>> source)
        where T : notnull
        => new CollectionNotifier<T>(source);
}

public class CollectionNotifier<T> : ExtendedDisposable, ICollectionNotifier
    where T : notnull
{
    public CollectionNotifier(IObservable<IChangeSet<T>> source)
        => source.Bind(out collection).Subscribe().DisposeWith(this);

    private readonly ReadOnlyObservableCollection<T> collection;

    private INotifyCollectionChanged Collection => collection;

    public event NotifyCollectionChangedEventHandler? CollectionChanged
    {
        add => Collection.CollectionChanged += value;
        remove => Collection.CollectionChanged -= value;
    }
}