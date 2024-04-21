namespace Markwardt;

public interface IDataIndexer
{
    ValueTask<IDataObject?> TryFindIndex(string index);
}

public static class DataIndexerExtensions
{
    public static async ValueTask<TEntity?> TryFindIndex<TEntity>(this IDataIndexer indexer, string index)
        where TEntity : IDataObject
        => (TEntity?) await indexer.TryFindIndex(index);

    public static async ValueTask<IDataObject> FindIndex(this IDataIndexer indexer, string index)
        => await indexer.TryFindIndex(index) ?? throw new InvalidOperationException($"No entity found for index {index}");
    
    public static async ValueTask<TEntity> FindIndex<TEntity>(this IDataIndexer indexer, string index)
        where TEntity : IDataObject
        => (TEntity) await indexer.FindIndex(index);
}