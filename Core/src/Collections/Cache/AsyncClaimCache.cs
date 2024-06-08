namespace Markwardt;

public interface IAsyncClaimCache<TKey, T>
{
    IObservable<IEnumerable<T>> Removed { get; }
    
    bool Contains(TKey key);
    ValueTask<IDictionary<TKey, IDisposable<T>>> Claim(IEnumerable<TKey> keys, bool requireAll = true);
}

public static class AsyncClaimCacheExtensions
{
    public static async ValueTask<Maybe<IDisposable<T>>> Claim<TKey, T>(this IAsyncClaimCache<TKey, T> cache, TKey key)
        => (await cache.Claim([key], false)).TryGetValue(key, out IDisposable<T>? value) ? value.Maybe() : default;
}

public class AsyncClaimCache<TKey, T>(IClaimCachePolicy<T> defaultPolicy, AsyncKeyLoader<TKey, T> load) : IAsyncClaimCache<TKey, T>
    where TKey : notnull
{
    private readonly ClaimCacheStore<TKey, T> cache = new(defaultPolicy);
    private readonly Dictionary<TKey, Task> currentLoads = [];

    public IObservable<IEnumerable<T>> Removed => cache.Removed;

    public async ValueTask<IDictionary<TKey, IDisposable<T>>> Claim(IEnumerable<TKey> keys, bool requireAll = true)
    {
        Dictionary<TKey, IDisposable<T>> claims = [];
        Dictionary<TKey, Task<IDisposable<T>>> dependentLoads = [];
        Dictionary<TKey, TaskCompletionSource> newLoads = [];

        async Task<IDisposable<T>> DependentLoad(TKey key, Task targetLoad)
        {
            await targetLoad;
            return cache.Claim(key).Value;
        }

        foreach (TKey key in keys)
        {
            if (cache.Claim(key).TryGetValue(out IDisposable<T>? cachedItem))
            {
                claims[key] = cachedItem;
            }
            else if (currentLoads.TryGetValue(key, out Task? currentLoad))
            {
                dependentLoads[key] = DependentLoad(key, currentLoad);
            }
            else
            {
                TaskCompletionSource newLoad = new();
                currentLoads[key] = newLoad.Task;
                newLoads[key] = newLoad;
            }
        }

        foreach (KeyValuePair<TKey, T> loadedItem in await load(newLoads.Keys))
        {
            claims[loadedItem.Key] = cache.SetAndClaim(loadedItem.Key, loadedItem.Value);
            newLoads[loadedItem.Key].SetResult();
            currentLoads.Remove(loadedItem.Key);
        }

        await Task.WhenAll(dependentLoads.Values);
        dependentLoads.ForEach(x => claims[x.Key] = x.Value.Result);

        if (requireAll && keys.All(claims.ContainsKey))
        {
            await claims.Values.TryDisposeAllAsync();
            throw new InvalidOperationException();
        }

        return claims;
    }

    public bool Contains(TKey key)
        => cache.Contains(key);
}