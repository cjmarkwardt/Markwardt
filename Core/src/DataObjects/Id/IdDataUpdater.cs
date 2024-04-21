namespace Markwardt;

public interface IIdDataUpdater
{
    void AddTo(IDataObject entity);
    void RemoveFrom(IDataObject entity);
}

public class IdDataUpdater(string sourceId, string targetProperty) : IIdDataUpdater
{
    public void AddTo(IDataObject entity)
        => GetTarget(entity).Add(sourceId);

    public void RemoveFrom(IDataObject entity)
        => GetTarget(entity).Remove(sourceId);

    private IIdDataInjector GetTarget(IDataObject entity)
        => entity.CastTo<IDataAccessor>().GetProperty(targetProperty).CastTo<IIdDataInjector>();
}