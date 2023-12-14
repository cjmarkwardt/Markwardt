namespace Markwardt;

public interface IIdDataLinkSet
{
    ISet<string> TargetIds { get; }
}

public class IdDataLinkSet<T>(IIdDataLoader loader, IIdDataUpdater? updater, IEnumerable<string> targetIds) : IDataLinkSet<T>, IIdDataLinkSet, IIdDataInjector
    where T : class, IDataObject
{
    public ISet<string> TargetIds { get; } = targetIds.ToHashSet();

    public bool Add(T entity)
    {
        updater?.AddTo(entity);
        return TargetIds.Add(entity.CastTo<IIdDataObject>().Id);
    }

    public bool Remove(T entity)
    {
        updater?.RemoveFrom(entity);
        return TargetIds.Remove(entity.CastTo<IIdDataObject>().Id);
    }

    public bool Contains(T entity)
        => TargetIds.Contains(entity.CastTo<IIdDataObject>().Id);

    public async ValueTask<bool> Clear()
    {
        bool hasItems = TargetIds.Count > 0;
        
        await foreach (T entity in this)
        {
            Remove(entity);
        }

        return hasItems;
    }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        foreach (string id in TargetIds.ToList())
        {
            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            IDataObject? entity = await loader.TryLoad(id);
            if (entity != null)
            {
                yield return (T) entity;
            }
        }
    }

    void IIdDataInjector.Add(string id)
        => TargetIds.Add(id);

    void IIdDataInjector.Remove(string id)
        => TargetIds.Remove(id);
}