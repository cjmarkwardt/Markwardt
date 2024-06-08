namespace Markwardt;

public interface IEntityLoader
{
    ValueTask<IEnumerable<IDisposable<IEntity>>> Load(IEnumerable<EntityId> ids);
}

public static class EntityLoaderExtensions
{
    public static async ValueTask<IDisposable<IEntity>> Load(this IEntityLoader loader, EntityId id)
        => (await loader.Load([id])).First();
}