namespace Markwardt;

public interface IClaimSource<TKey>
{
    IAsyncEnumerable<KeyValuePair<TKey, Failable<IDisposable<T>>>> Claim<T>(IEnumerable<TKey> keys);
    IObservable Watch(TKey key);
}

public static class ClaimSourceExtensions
{
    public static async ValueTask<Failable<IDisposable<T>>> Claim<TKey, T>(this IClaimSource<TKey> source, TKey key)
        => await source.Claim<T>([key]).Select(x => x.Value).FirstAsync();

    public static IAsyncEnumerable<KeyValuePair<TKey, Failable<IDisposable<TSelected>>>> Claim<TKey, T, TSelected>(this IClaimSource<TKey> source, IEnumerable<TKey> keys, Func<T, TSelected> selector)
        where TKey : notnull
        => source.Claim<T>(keys).Select(x => new KeyValuePair<TKey, Failable<IDisposable<TSelected>>>(x.Key, x.Value.Convert(y => y.Select(selector))));

    public static async ValueTask<Failable<IDisposable<TSelected>>> Claim<TKey, T, TSelected>(this IClaimSource<TKey> source, TKey key, Func<T, TSelected> selector)
        where TKey : notnull
        => await source.Claim([key], selector).Select(x => x.Value).FirstAsync();
        
    public static async IAsyncEnumerable<KeyValuePair<TKey, Failable<TResult>>> Access<TKey, T, TResult>(this IClaimSource<TKey> source, IEnumerable<TKey> keys, Func<T, ValueTask<TResult>> access)
    {
        await foreach (KeyValuePair<TKey, Failable<IDisposable<T>>> claim in source.Claim<T>(keys))
        {
            if (claim.Value.Exception is not null)
            {
                yield return new(claim.Key, claim.Value.Exception);
            }
            else
            {
                await using IDisposable<T> target = claim.Value.Result;
                yield return new(claim.Key, await access(target.Value));
            }
        }
    }
    
    public static IAsyncEnumerable<KeyValuePair<TKey, Failable<TResult>>> Access<TKey, T, TResult>(this IClaimSource<TKey> source, IEnumerable<TKey> keys, Func<T, TResult> access)
        => source.Access(keys, (T x) => ValueTask.FromResult(access(x)));

    public static async ValueTask<Failable<TResult>> Access<TKey, T, TResult>(this IClaimSource<TKey> source, TKey key, Func<T, ValueTask<TResult>> access)
        => await source.Access([key], access).Select(x => x.Value).FirstAsync();
    
    public static async ValueTask<Failable<TResult>> Access<TKey, T, TResult>(this IClaimSource<TKey> source, TKey key, Func<T, TResult> access)
        => await source.Access([key], access).Select(x => x.Value).FirstAsync();

    public static IAsyncEnumerable<KeyValuePair<TKey, Failable<T>>> Access<TKey, T>(this IClaimSource<TKey> source, IEnumerable<TKey> keys)
        => source.Access(keys, (T x) => x);

    public static async ValueTask<Failable<T>> Access<TKey, T>(this IClaimSource<TKey> source, TKey key)
        => await source.Access([key], (T x) => x).Select(x => x.Value).FirstAsync();
    
    public static IAsyncEnumerable<KeyValuePair<TKey, Failable>> Access<TKey, T>(this IClaimSource<TKey> source, IEnumerable<TKey> keys, Func<T, ValueTask> access)
        => source.Access<TKey, T, object?>(keys, async (T x) => { await access(x); return null; }).Select(x => new KeyValuePair<TKey, Failable>(x.Key, x.Value));
    
    public static IAsyncEnumerable<KeyValuePair<TKey, Failable>> Access<TKey, T>(this IClaimSource<TKey> source, IEnumerable<TKey> keys, Action<T> access)
        => source.Access<TKey, T, object?>(keys, (T x) => { access(x); return null; }).Select(x => new KeyValuePair<TKey, Failable>(x.Key, x.Value));

    public static async ValueTask<Failable> Access<TKey, T>(this IClaimSource<TKey> source, TKey key, Func<T, ValueTask> access)
        => await source.Access<TKey, T, object?>(key, async (T x) => { await access(x); return null; });
    
    public static async ValueTask<Failable> Access<TKey, T>(this IClaimSource<TKey> source, TKey key, Action<T> access)
        => await source.Access<TKey, T, object?>(key, (T x) => { access(x); return null; });
}