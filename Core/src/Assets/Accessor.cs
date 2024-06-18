namespace Markwardt;

public interface IAccessor
{
    IAsyncEnumerable<KeyValuePair<string, Failable<TResult>>> Access<T, TResult>(IEnumerable<string> ids, Func<T, ValueTask<TResult>> access);
    IObservable Watch(string id);
}

public static class AccessorExtensions
{
    public static async ValueTask<IEnumerable<KeyValuePair<string, Failable<TResult>>>> Access<T, TResult>(this IAccessor accessor, IEnumerable<string> ids, Func<T, TResult> access)
        => await accessor.Access(ids, (T x) => ValueTask.FromResult(access(x)));

    public static async ValueTask<Failable<TResult>> Access<T, TResult>(this IAccessor accessor, string id, Func<T, ValueTask<TResult>> access)
        => (await accessor.Access([id], access)).MaybeFirst().Select(x => x.Value);
    
    public static async ValueTask<Failable<TResult>> Access<T, TResult>(this IAccessor accessor, string id, Func<T, TResult> access)
        => (await accessor.Access([id], access)).MaybeFirst().Select(x => x.Value);

    public static async ValueTask<IEnumerable<KeyValuePair<string, Failable<T>>>> Access<T>(this IAccessor accessor, IEnumerable<string> ids)
        => await accessor.Access(ids, (T x) => x);

    public static async ValueTask<Maybe<T>> Access<T>(this IAccessor accessor, string id)
        => await accessor.Access(id, (T x) => x);
    
    public static async ValueTask Access<T>(this IAccessor accessor, IEnumerable<string> ids, Func<T, ValueTask> access)
        => await accessor.Access<T, object?>(ids, async (T x) => { await access(x); return null; });
    
    public static async ValueTask Access<T>(this IAccessor accessor, IEnumerable<string> ids, Action<T> access)
        => await accessor.Access<T, object?>(ids, (T x) => { access(x); return null; });

    public static async ValueTask Access<T>(this IAccessor accessor, string id, Func<T, ValueTask> access)
        => await accessor.Access<T, object?>(id, async (T x) => { await access(x); return null; });
    
    public static async ValueTask Access<T>(this IAccessor accessor, string id, Action<T> access)
        => await accessor.Access<T, object?>(id, (T x) => { access(x); return null; });
}