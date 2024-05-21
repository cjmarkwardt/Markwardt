namespace Markwardt;

public static class EntityIdExtensions
{
    public static async ValueTask<IEntityClaim> Load(this EntityId id, IEntityLoader loader)
        => await loader.Load(id);

    public static async ValueTask<IEnumerable<IEntityClaim>> Load(this IEnumerable<EntityId> ids, IEntityLoader loader)
        => await loader.Load(ids.ToList());
}

public record struct EntityId(string Value)
{
    public override readonly string ToString()
        => Value;
}