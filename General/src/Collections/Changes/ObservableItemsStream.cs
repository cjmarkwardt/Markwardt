using DynamicData;

namespace Markwardt;

public static class ObservableItemsStreamExtensions
{
    public static IObservable<T> AsAdds<T>(this IObservable<ICollectionChange<T>> source)
        where T : notnull
        => source.Where(x => x.ChangeType is CollectionChangeType.Add).SelectMany(x => x.Items);

    public static IObservable<T> AsRemoves<T>(this IObservable<ICollectionChange<T>> source)
        where T : notnull
        => source.Where(x => x.ChangeType is CollectionChangeType.Remove).SelectMany(x => x.Items);
        
    public static IObservable AsClears<T>(this IObservable<ICollectionChange<T>> source)
        where T : notnull
        => source.Where(x => x.ChangeType is CollectionChangeType.Clear).Generalize();
}

public abstract class ObservableItemsStream<T>(IObservable<IChangeSet<T>> source) : IObservable<ICollectionChange<T>>
    where T : notnull
{
    protected IObservable<IChangeSet<T>> Source { get; } = source;

    public IDisposable Subscribe(IObserver<ICollectionChange<T>> observer)
        => Source.SelectMany(x =>
        {
            List<ICollectionChange<T>> changes = [];
            foreach (Change<T> item in x)
            {
                if (item.Reason is ListChangeReason.Add)
                {
                    changes.Add(CollectionChange.Add([item.Item.Current]));
                }
                else if (item.Reason is ListChangeReason.AddRange)
                {
                    changes.Add(CollectionChange.Add(item.Range));
                }
                else if (item.Reason is ListChangeReason.Remove)
                {
                    changes.Add(CollectionChange.Remove([item.Item.Current]));
                }
                else if (item.Reason is ListChangeReason.RemoveRange)
                {
                    changes.Add(CollectionChange.Remove(item.Range));
                }
                else if (item.Reason is ListChangeReason.Replace)
                {
                    changes.Add(CollectionChange.Remove([item.Item.Previous.Value]));
                    changes.Add(CollectionChange.Add([item.Item.Current]));
                }
                else if (item.Reason is ListChangeReason.Clear)
                {
                    changes.Add(CollectionChange.Clear<T>());
                }
            }

            return changes;
        }).Subscribe(observer);
}