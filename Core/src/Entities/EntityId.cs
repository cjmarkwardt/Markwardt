namespace Markwardt;

public static class EntityIdExtensions
{
    public static async ValueTask<IEntityClaim> Load(this EntityId id, IEntityLoader loader)
        => await loader.Load(id.Value);

    public static async ValueTask<IEnumerable<IEntityClaim>> Load(this IEnumerable<EntityId> ids, IEntityLoader loader)
        => await loader.Load(ids.Select(x => x.Value).ToList());
}

public record struct EntityId(string Value)
{
    public static implicit operator EntityId(string value)
        => new(value);

    public static implicit operator string(EntityId id)
        => id.Value;

    public override string ToString()
        => Value;
}