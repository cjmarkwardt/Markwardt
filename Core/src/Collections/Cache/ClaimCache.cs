namespace Markwardt;

public interface IClaimCache<TKey, T> : IClaimSource<TKey, T>, IMultiDisposable
{
    IReadOnlyDictionary<TKey, T> Current { get; }

    IObservable<IEnumerable<T>> Purged { get; }

    void Purge();
}

public class ClaimCache<TKey, T> : ExtendedDisposable, IClaimCache<TKey, T>
    where TKey : notnull
{
    public ClaimCache(IClaimCachePolicy<TKey, T> policy, IKeyLoader<TKey, T> loader)
    {
        this.policy = policy;
        this.loader = loader;

        Current = new EntryViewer(entries);
    }

    private readonly IClaimCachePolicy<TKey, T> policy;
    private readonly IKeyLoader<TKey, T> loader;
    private readonly Dictionary<TKey, Entry> entries = [];
    private readonly Dictionary<TKey, Task> currentLoads = [];
    private readonly AsyncStatus claiming = new();

    private readonly Subject<IEnumerable<T>> purged = new();
    public IObservable<IEnumerable<T>> Purged => purged;

    public IReadOnlyDictionary<TKey, T> Current { get; }

    public async IAsyncEnumerable<KeyValuePair<TKey, IDisposable<T>>> Claim(IEnumerable<TKey> keys)
    {
        if (this.IsDisposed())
        {
            yield break;
        }

        using IDisposable claimingStatus = claiming.Start();

        HashSet<Task<KeyValuePair<TKey, IDisposable<T>>>> dependentLoads = [];
        Dictionary<TKey, TaskCompletionSource> newLoads = [];

        async Task<KeyValuePair<TKey, IDisposable<T>>> WaitForLoad(TKey key, Task targetLoad)
        {
            await targetLoad;
            return new(key, entries[key].Claim());
        }

        foreach (TKey key in keys)
        {
            if (entries.TryGetValue(key, out Entry? entry))
            {
                yield return new(key, entry.Claim());
            }
            else if (currentLoads.TryGetValue(key, out Task? currentLoad))
            {
                dependentLoads.Add(WaitForLoad(key, currentLoad));
            }
            else
            {
                newLoads[key] = StartLoad(key);
            }
        }

        IAsyncEnumerable<KeyValuePair<TKey, IDisposable<T>>> loadResults = dependentLoads.AwaitMerge();

        if (newLoads.Count > 0)
        {
            loadResults.Merge(loader.Load(newLoads.Keys).Select(x => new KeyValuePair<TKey, IDisposable<T>>(x.Key, EndLoad(x.Key, x.Value))).OnEach(x => newLoads[x.Key].SetResult()));
        }

        await foreach (KeyValuePair<TKey, IDisposable<T>> loadResult in loadResults)
        {
            yield return loadResult;
        }
    }

    public void Purge()
    {
        if (!this.IsDisposed())
        {
            Purge(entries.Where(x => x.Value.IsExpired(policy)).ToList());
        }
    }

    protected override void OnDisposal()
    {
        base.OnDisposal();

        Purge(entries);
    }

    protected override async ValueTask OnAsyncDisposal()
    {
        await base.OnAsyncDisposal();

        await claiming.Task;
        Purge(entries);
    }

    private void Purge(IEnumerable<KeyValuePair<TKey, Entry>> entries)
    {
        if (entries.Any())
        {
            entries.ForEach(x => this.entries.Remove(x.Key));
            purged.OnNext(entries.Select(x => x.Value.Item));
        }
    }

    private TaskCompletionSource StartLoad(TKey key)
    {
        TaskCompletionSource load = new();
        currentLoads[key] = load.Task;
        return load;
    }

    private IDisposable<T> EndLoad(TKey key, T item)
    {
        Entry entry = new(key, item);
        entries.Add(key, entry);
        currentLoads.Remove(key);
        return entry.Claim();
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