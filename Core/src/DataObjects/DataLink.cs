namespace Markwardt;

public interface IDataLink<T>
    where T : class?, IDataObject
{
    ValueTask<T> Get();
    ValueTask<bool> Set(T entity);
}