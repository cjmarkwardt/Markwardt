namespace Markwardt;

[Singleton<EntityIdCreator>]
public interface IEntityIdCreator
{
    string Create(string? id = null);
}

public class EntityIdCreator : IEntityIdCreator
{
    public string Create(string? id = null)
        => id is not null ? $"@{id}" : $"#{Guid.NewGuid():N}";
}