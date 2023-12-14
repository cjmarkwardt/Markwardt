namespace Markwardt;

public interface IIdDataLoader
{
    ValueTask<IDataObject?> TryLoad(string id);
}

public static class IdDataLoaderExtensions
{
    public static async ValueTask<TEntity?> TryLoad<TEntity>(this IIdDataLoader loader, string id)
        where TEntity : IDataObject
        => (TEntity?) await loader.TryLoad(id);

    public static async ValueTask<IDataObject> Load(this IIdDataLoader loader, string id)
        => await loader.TryLoad(id) ?? throw new InvalidOperationException($"No object found with id {id}");
    
    public static async ValueTask<TEntity> Load<TEntity>(this IIdDataLoader loader, string id)
        where TEntity : IDataObject
        => (TEntity) await loader.Load(id);
}