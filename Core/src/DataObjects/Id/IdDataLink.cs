namespace Markwardt;

public interface IIdDataLink
{
    string? TargetId { get; set; }
}

public class IdDataLink<T>(IIdDataLoader loader, IIdDataUpdater? updater, string? targetId) : IDataLink<T>, IIdDataLink, IIdDataInjector
    where T : class?, IDataObject
{
    public string? TargetId { get; set; } = targetId;

    public async ValueTask<T> Get()
        => (TargetId is null ? null : await loader.TryLoad<T>(TargetId))!;

    public async ValueTask<bool> Set(T entity)
    {
        string? id = entity?.CastTo<IIdDataObject>().Id;
        if (id == TargetId)
        {
            return false;
        }
        
        if (TargetId != null && updater != null)
        {
            T? previous = await loader.TryLoad<T>(TargetId);
            if (previous != null)
            {
                updater.RemoveFrom(previous);
            }
        }

        TargetId = id;

        if (entity != null && updater != null)
        {
            updater.AddTo(entity);
        }

        return false;
    }

    void IIdDataInjector.Add(string id)
        => TargetId = id;

    void IIdDataInjector.Remove(string id)
    {
        if (TargetId == id)
        {
            TargetId = null;
        }
    }
}