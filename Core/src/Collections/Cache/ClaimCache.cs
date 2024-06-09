namespace Markwardt;

public interface IClaimCache<in TRequest, TKey, T> : IMultiDisposable
{
    IReadOnlyDictionary<TKey, T> Current { get; }

    IObservable<IEnumerable<T>> Purged { get; }

    ValueTask<IReadOnlyDictionary<TKey, IDisposable<T>>> Claim(TRequest request, bool requireAll = true);
    void Purge();
}

public static class ClaimCacheExtensions
{
    public static async ValueTask<Maybe<IDisposable<T>>> Claim<TKey, T>(this IClaimCache<IEnumerable<TKey>, TKey, T> cache, TKey key)
        => (await cache.Claim([key], false)).TryGetValue(key, out IDisposable<T>? value) ? value.Maybe() : default;
}

public class ClaimCache<TRequest, TKey, T> : ExtendedDisposable, IClaimCache<TRequest, TKey, T>
    where TKey : notnull
{
    public ClaimCache(IClaimCachePolicy<TKey, T> policy, ICacheKeyRequester<TRequest, TKey> requester, ICacheKeyLoader<TKey, T> loader)
    {
        this.policy = policy;
        this.requester = requester;
        this.loader = loader;

        Current = new EntryViewer(entries);
    }

    private readonly IClaimCachePolicy<TKey, T> policy;
    private readonly ICacheKeyRequester<TRequest, TKey> requester;
    private readonly ICacheKeyLoader<TKey, T> loader;
    private readonly Dictionary<TKey, Entry> entries = [];
    private readonly Dictionary<TKey, Task> currentLoads = [];
    private readonly AsyncStatus claiming = new();

    private readonly Subject<IEnumerable<T>> purged = new();
    public IObservable<IEnumerable<T>> Purged => purged;

    public IReadOnlyDictionary<TKey, T> Current { get; }

    public async ValueTask<IReadOnlyDictionary<TKey, IDisposable<T>>> Claim(TRequest request, bool requireAll = true)
    {
        this.VerifyUndisposed();
        using IDisposable claimingStatus = claiming.Start();

        Dictionary<TKey, IDisposable<T>> claims = [];
        Dictionary<TKey, Task<IDisposable<T>>> dependentLoads = [];
        Dictionary<TKey, TaskCompletionSource> newLoads = [];

        async Task<IDisposable<T>> DependentLoad(TKey key, Task targetLoad)
        {
            await targetLoad;
            return entries[key].Claim();
        }

        IEnumerable<TKey> keys = await requester.Request(request);

        foreach (TKey key in keys)
        {
            if (entries.TryGetValue(key, out Entry? entry))
            {
                claims[key] = entry.Claim();
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

        if (newLoads.Count > 0)
        {
            IDictionary<TKey, T> newItems = await loader.Load(newLoads.Keys);

            foreach (KeyValuePair<TKey, T> load in newItems)
            {
                Entry entry = new(load.Key, load.Value);
                entries.Add(load.Key, entry);
                claims.Add(load.Key, entry.Claim());
                currentLoads.Remove(load.Key);
            }

            newItems.Keys.ForEach(x => newLoads[x].SetResult());
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

    public void Purge()
    {
        if (!this.IsDisposed())
        {
            Purge(entries.Where(x => x.Value.IsExpired(policy)).ToList());
        }
    }

    protected override void OnSharedDisposal()
    {
        base.OnSharedDisposal();

        TaskExtensions.Fork(async () =>
        {
            await claiming.Task;
            Purge(entries);
        });
    }

    private void Purge(IEnumerable<KeyValuePair<TKey, Entry>> entries)
    {
        if (entries.Any())
        {
            entries.ForEach(x => this.entries.Remove(x.Key));
            purged.OnNext(entries.Select(x => x.Value.Item));
        }
    }

    private sealed class Entry(TKey key, T item)
    {
        public T Item => item;

        private int claims;
        private DateTime lastClaim = DateTime.Now;
        private DateTime? lastRelease;

        public IDisposable<T> Claim()
        {
            claims++;
            lastClaim = DateTime.Now;

            void Destroy()
            {
                claims--;
                if (claims == 0)
                {
                    lastRelease = DateTime.Now;
                }
            }

            return new Disposable<T>(item, [new Finalizer(Destroy)]);
        }

        public bool IsExpired(IClaimCachePolicy<TKey, T> policy)
            => policy.IsExpired(key, item, claims, lastClaim, lastRelease);
    }

    private sealed class EntryViewer(Dictionary<TKey, Entry> source) : IReadOnlyDictionary<TKey, T>
    {
        public T this[TKey key] => source[key].Item;

        public IEnumerable<TKey> Keys => source.Keys;

        public IEnumerable<T> Values => source.Values.Select(x => x.Item);

        public int Count => source.Count;

        public bool ContainsKey(TKey key)
            => source.ContainsKey(key);

        public IEnumerator<KeyValuePair<TKey, T>> GetEnumerator()
            => source.Select(x => new KeyValuePair<TKey, T>(x.Key, x.Value.Item)).GetEnumerator();

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out T value)
        {
            if (source.TryGetValue(key, out Entry? entry))
            {
                value = entry.Item;
                return true;
            }

            value = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}

public class ClaimCache<TKey, T>(IClaimCachePolicy<TKey, T> policy, ICacheKeyLoader<TKey, T> loader) : ClaimCache<IEnumerable<TKey>, TKey, T>(policy, new CacheKeyRequester<IEnumerable<TKey>, TKey>(x => ValueTask.FromResult(x)), loader)
    where TKey : notnull;