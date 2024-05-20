namespace Markwardt;

public interface IEntityLoader
{
    ValueTask<IEnumerable<IEntityClaim>> Load(IEnumerable<string> ids);
}

public static class EntityLoaderExtensions
{
    public static async ValueTask<IEntityClaim> Load(this IEntityLoader loader, string id)
        => (await loader.Load([id])).First();
}