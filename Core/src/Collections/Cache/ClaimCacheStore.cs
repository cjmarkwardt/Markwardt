namespace Markwardt;

public interface IClaimCacheStore<TKey, T> : IClaimCache<TKey, T>
{
    IObservable<IEnumerable<T>> Removed { get; }

    void Set(TKey key, T item, IClaimCachePolicy<T>? policy = null);
    bool Remove(TKey key);
    bool Clear();
    void Purge();
}