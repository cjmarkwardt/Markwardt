namespace Markwardt;

public interface IEntityLoader
{
    ValueTask<IEnumerable<IEntityClaim>> Load(IEnumerable<EntityId> ids);
}

public static class EntityLoaderExtensions
{
    public static async ValueTask<IEntityClaim> Load(this IEntityLoader loader, EntityId id)
        => (await loader.Load([id])).First();
}