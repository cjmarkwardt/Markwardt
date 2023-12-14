namespace Markwardt;

public interface IDataLinkSet<T> : IAsyncEnumerable<T>
    where T : class, IDataObject
{
    bool Add(T entity);
    bool Remove(T entity);
    bool Contains(T entity);
    
    ValueTask<bool> Clear();
}