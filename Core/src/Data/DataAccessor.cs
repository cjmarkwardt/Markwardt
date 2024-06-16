namespace Markwardt;

public interface IDataAccessor<TKey, TData>
{
    ValueTask Access(TKey key, Func<TData, ValueTask> read);
}

public static class DataAccessorExtensions
{
    public static async ValueTask<TResult> Read<TKey, TData, TResult>(this IDataAccessor<TKey, TData> accessor, TKey key, Func<TData, ValueTask<TResult>> read)
    {
        TResult result = default!;
        await accessor.Access(key, async data => result = await read(data));
        return result;
    }
}