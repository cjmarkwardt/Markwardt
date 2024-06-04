namespace Markwardt;

public interface IClaimCache<TKey, T>
{
    bool Contains(TKey key);
    Maybe<IDisposable<T>> Claim(TKey key);
}

public class ClaimCache<TKey, T>(IClaimCachePolicy<T> defaultPolicy) : IClaimCacheStore<TKey, T>
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

    public Maybe<IDisposable<T>> Claim(TKey key)
    {
        if (entries.TryGetValue(key, out Entry? entry))
        {
            return entry.Claim().Maybe();
        }

        return default;
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
            return new Disposable<T>(item, [new Finalizer(() => claims--)]);
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