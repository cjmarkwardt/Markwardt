namespace Markwardt;

public interface IObservableItems<T>
    where T : notnull
{
    IObservable<IChangeSet<T>> Observe();
}

public static class ObservableItemsExtensions
{
    public static IObservable<ICollectionChange<T>> AsChanges<T>(this IObservable<IChangeSet<T>> changes)
        where T : notnull
        => changes.SelectMany(x =>
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
                    changes.Add(CollectionChange.Remove(item.Range));
                }
            }

            return changes;
        });

    public static IObservable<T> AsAdds<T>(this IObservable<ICollectionChange<T>> changes)
        where T : notnull
        => changes.Where(x => x.ChangeType is CollectionChangeType.Add).SelectMany(x => x.Items);

    public static IObservable<T> AsAdds<T>(this IObservable<IChangeSet<T>> source)
        where T : notnull
        => source.AsChanges().AsAdds();

    public static IObservable<T> AsRemoves<T>(this IObservable<ICollectionChange<T>> changes)
        where T : notnull
        => changes.Where(x => x.ChangeType is CollectionChangeType.Remove).SelectMany(x => x.Items);

    public static IObservable<T> AsRemoves<T>(this IObservable<IChangeSet<T>> source)
        where T : notnull
        => source.AsChanges().AsRemoves();

    public static IDisposable SubscribeChanges<T>(this IObservable<ICollectionChange<T>> changes, Action<T> onAdd, Action<T> onRemove)
        where T : notnull
    {
        IDisposable addSubscription = changes.AsAdds().Subscribe(onAdd);
        IDisposable removeSubscription = changes.AsRemoves().Subscribe(onRemove);
        return Disposable.Create(() =>
        {
            addSubscription.Dispose();
            removeSubscription.Dispose();
        });
    }

    public static IDisposable SubscribeChanges<T>(this IObservable<IChangeSet<T>> changes, Action<T> onAdd, Action<T> onRemove)
        where T : notnull
        => changes.AsChanges().SubscribeChanges(onAdd, onRemove);

    public static IDisposable OutputToDictionary<T, TKey, TValue>(this IObservableItems<T> changes, IDictionary<TKey, TValue> dictionary, Func<T, TKey> getKey, Func<T, TValue> getValue)
        where T : notnull
        => changes.Observe().SubscribeChanges(x => dictionary.Add(getKey(x), getValue(x)), x => dictionary.Remove(getKey(x)));

    public static IDisposable OutputToDictionary<T, TKey>(this IObservableItems<T> changes, IDictionary<TKey, T> dictionary, Func<T, TKey> getKey)
        where T : notnull
        => changes.OutputToDictionary(dictionary, getKey, x => x);

    public static IObservableReadOnlyCollection<T> ToCollection<T>(this IObservable<IChangeSet<T>> changes)
        where T : notnull
        => changes.ToList();

    public static IObservableReadOnlyList<T> ToList<T>(this IObservable<IChangeSet<T>> changes)
        where T : notnull
        => new ObservableReadOnlyList<T>(changes.AsObservableList());
    
    public static IObservableReadOnlyCache<TKey, T> ToCache<TKey, T>(this IObservable<IChangeSet<T>> changes, Func<T, TKey> keySelector)
        where TKey : notnull
        where T : notnull
        => new ObservableReadOnlyCache<TKey, T>(changes.AsObservableList(), keySelector);

    public static IObservableReadOnlyDictionary<TKey, T> ToMap<TKey, T>(this IObservable<IChangeSet<KeyValuePair<TKey, T>>> changes)
        where TKey : notnull
        where T : notnull
        => new ObservableReadOnlyDictionary<TKey, T>(changes.AsObservableList());
}