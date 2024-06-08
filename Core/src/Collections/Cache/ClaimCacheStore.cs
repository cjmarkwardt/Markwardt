namespace Markwardt;

public interface IClaimCacheStore<TKey, T> : IClaimCache<TKey, T>
{
    void Set(TKey key, T item, IClaimCachePolicy<T>? policy = null);
    bool Remove(TKey key);
    bool Clear();
    void Purge();
}

public static class ClaimCacheStoreExtensions
{
    public static IDisposable<T> SetAndClaim<TKey, T>(this IClaimCacheStore<TKey, T> cache, TKey key, T item, IClaimCachePolicy<T>? policy = null)
    {
        cache.Set(key, item, policy);
        return cache.Claim(key).Value;
    }
}

public class ClaimCacheStore<TKey, T>(IClaimCachePolicy<T> defaultPolicy) : IClaimCacheStore<TKey, T>
    where TKey : notnull
{
    private readonly Dictionary<TKey, Entry> entries = [];

    private readonly Subject<IEnumerable<T>> removed = new();
    public IObservable<IEnumerable<T>> Removed => removed;

    public bool Clear()
    {
        if (entries.Count > 0)
        {
            IEnumerable<T> removedItems = entries.Select(x => x.Value.Item).ToList();
            entries.Clear();
            removed.OnNext(removedItems);
            return true;
        }

        return false;
    }

    public bool Contains(TKey key)
        => entries.ContainsKey(key);

    public IDictionary<TKey, IDisposable<T>> Claim(IEnumerable<TKey> keys, bool requireAll = true)
    {
        if (requireAll && keys.All(entries.ContainsKey))
        {
            throw new InvalidOperationException();
        }

        Dictionary<TKey, IDisposable<T>> claims = [];
        foreach (TKey key in keys)
        {
            if (entries.TryGetValue(key, out Entry? entry))
            {
                claims[key] = entry.Claim();
            }
        }

        return claims;
    }

    public void Purge()
    {
        IEnumerable<Entry> purgedEntries = entries.Values.Where(x => x.IsExpired()).ToList();
        purgedEntries.ForEach(x => entries.Remove(x.Key));
        removed.OnNext(purgedEntries.Select(x => x.Item));
    }

    public bool Remove(TKey key)
    {
        if (entries.Remove(key, out Entry? entry))
        {
            removed.OnNext([entry.Item]);
            return true;
        }

        return false;
    }

    public void Set(TKey key, T item, IClaimCachePolicy<T>? policy = null)
    {
        Remove(key);
        entries.Add(key, new(key, item, policy ?? defaultPolicy));
    }

    private sealed class Entry(TKey key, T item, IClaimCachePolicy<T> policy)
    {
        public TKey Key => key;
        public T Item => item;

        private int claims;
        private DateTime lastClaim = DateTime.Now;
        private DateTime? lastRelease;

        public IDisposable<T> Claim()
        {
            claims++;
            lastClaim = DateTime.Now;
            return new Disposable<T>(item, [new Finalizer(DestroyClaim)]);
        }

        public bool IsExpired()
            => policy.IsExpired(item, claims, lastClaim, lastRelease);

        private void DestroyClaim()
        {
            claims--;

            if (claims == 0)
            {
                lastRelease = DateTime.Now;
            }
        }
    }
}