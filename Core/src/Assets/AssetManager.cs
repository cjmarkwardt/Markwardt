namespace Markwardt;

[Singleton<AssetManager>]
public interface IAssetManager : IAssetSource
{
    ISet<IAssetLoader> Loaders { get; }
    IReadOnlyDictionary<string, IAsset> Assets { get; }

    string? GetKey(object assetValue);
    void Preload(IEnumerable<string> keys);
}

public static class AssetManagerExtensions
{
    public static void ForceExpire(this IAssetManager manager, IEnumerable<string> keys, bool forceExpire = true)
    {
        foreach (string key in keys)
        {
            if (manager.Assets.TryGetValue(key, out IAsset? asset))
            {
                asset.ForceExpire = forceExpire;
            }
        }
    }

    public static void ForceExpire(this IAssetManager manager, string key, bool forceExpire = true)
        => manager.ForceExpire([key], forceExpire);
}

public class AssetManager : ExtendedDisposable, IAssetManager
{
    public AssetManager(IAssetPolicy policy)
    {
        this.policy = policy;

        this.LoopInBackground(async cancellation =>
        {
            IEnumerable<KeyValuePair<string, IAsset>> expiredAssets = assets.Where(x => x.Value.IsExpired).ToArray();
            expiredAssets.ForEach(x => assets.Remove(x.Key));
            await expiredAssets.ForEachParallel(async x => await x.Value.TryDisposeAsync());
        });
    }

    private readonly IAssetPolicy policy;
    private readonly ConditionalWeakTable<object, string> keys = [];
    private readonly Dictionary<string, Task<Failable<IAsset>>> currentLoads = [];

    public ISet<IAssetLoader> Loaders { get; } = new HashSet<IAssetLoader>();

    private readonly Dictionary<string, IAsset> assets = [];
    public IReadOnlyDictionary<string, IAsset> Assets => assets;

    public async IAsyncEnumerable<KeyValuePair<string, Failable<IDisposable<object>>>> Claim(IEnumerable<string> keys)
    {
        HashSet<Task<KeyValuePair<string, Failable<IDisposable<object>>>>> dependentLoads = [];
        Dictionary<string, TaskCompletionSource<Failable<IAsset>>> newLoads = [];

        static async Task<KeyValuePair<string, Failable<IDisposable<object>>>> WaitForLoad(string key, Task<Failable<IAsset>> targetLoad)
        {
            Failable<IAsset> tryAsset = await targetLoad;
            return new KeyValuePair<string, Failable<IDisposable<object>>>(key, tryAsset.Exception is not null ? tryAsset.Exception : tryAsset.Result.Claim().AsFailable());
        }

        foreach (string key in keys)
        {
            if (assets.TryGetValue(key, out IAsset? asset))
            {
                yield return new(key, asset.Claim().AsFailable());
            }
            else if (currentLoads.TryGetValue(key, out Task<Failable<IAsset>>? currentLoad))
            {
                dependentLoads.Add(WaitForLoad(key, currentLoad));
            }
            else
            {
                TaskCompletionSource<Failable<IAsset>> newLoad = new();
                currentLoads[key] = newLoad.Task;
                newLoads[key] = newLoad;
            }
        }
        
        await foreach (KeyValuePair<string, Failable<IDisposable<object>>> result in Load(newLoads).Merge(dependentLoads.AwaitMerge()))
        {
            yield return result;
        }
    }

    public string? GetKey(object assetValue)
        => keys.TryGetValue(assetValue, out string? key) ? key : null;

    public void Preload(IEnumerable<string> keys)
        => this.RunInBackground(async cancellation => await Claim(keys).ForEachAsync(x =>
        {
            if (x.Value.Exception is not null)
            {
                x.Value.Result.Dispose();
            }
        }));
    
    private async IAsyncEnumerable<KeyValuePair<string, Failable<IDisposable<object>>>> Load(IDictionary<string, TaskCompletionSource<Failable<IAsset>>> newLoads)
    {
        IMaybeEnumerator<IAssetLoader> loaders = Loaders.ToArray().GetMaybeEnumerator();
        while (newLoads.Count > 0 && loaders.Current.HasValue)
        {
            if (Loaders.Contains(loaders.Current.Value))
            {
                await foreach (KeyValuePair<string, Failable<IDisposable<object>>> tryLoad in Load(loaders.Current.Value, newLoads))
                {
                    yield return tryLoad;
                }
            }

            loaders.Next();
        }
    }
    
    private async IAsyncEnumerable<KeyValuePair<string, Failable<IDisposable<object>>>> Load(IAssetLoader loader, IDictionary<string, TaskCompletionSource<Failable<IAsset>>> newLoads)
    {
        await foreach (KeyValuePair<string, Failable<object>> tryLoad in loader.Load(loader.FilterHandled(newLoads.Keys)))
        {
            TaskCompletionSource<Failable<IAsset>> completion = newLoads[tryLoad.Key];
            newLoads.Remove(tryLoad.Key);
            currentLoads.Remove(tryLoad.Key);

            Failable<IAsset> tryAsset;
            if (tryLoad.Value.Exception is not null)
            {
                tryAsset = tryLoad.Value.Exception;
            }
            else
            {
                tryAsset = new Asset(this, tryLoad.Key, tryLoad.Value);
                assets.Add(tryLoad.Key, tryAsset.Result);
                keys.Add(tryAsset.Result, tryLoad.Key);
            }

            yield return new KeyValuePair<string, Failable<IDisposable<object>>>(tryLoad.Key, tryAsset.Exception is not null ? tryAsset.Exception : tryAsset.Result.Claim().AsFailable());

            completion.SetResult(tryAsset);
        }
    }

    private sealed class Asset(AssetManager manager, string key, object value) : IAsset
    {
        public string Key => key;
        public object Value => value;
        public bool IsExpired => claims == 0 && (ForceExpire || manager.policy.IsExpired(Key, Value, lastClaim, lastRelease));

        public bool ForceExpire { get; set; }
        
        private int claims;
        private DateTime lastClaim = DateTime.Now;
        private DateTime? lastRelease;

        public IDisposable<object> Claim()
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

            return new Disposable<object>(Value, [new Finalizer(Destroy)]);
        }
    }
}