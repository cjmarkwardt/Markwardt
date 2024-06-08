namespace Markwardt;

public interface IClaimCache<TKey, T>
{
    IObservable<IEnumerable<T>> Removed { get; }
    
    bool Contains(TKey key);
    IDictionary<TKey, IDisposable<T>> Claim(IEnumerable<TKey> keys, bool requireAll = true);
}

public static class ClaimCacheExtensions
{
    public static Maybe<IDisposable<T>> Claim<TKey, T>(this IClaimCache<TKey, T> cache, TKey key)
        => cache.Claim([key], false).TryGetValue(key, out IDisposable<T>? value) ? value.Maybe() : default;
}