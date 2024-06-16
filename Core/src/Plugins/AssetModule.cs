namespace Markwardt;

public interface IAssetModule : IClaimSource<AssetId, object>, IMultiDisposable
{
    [Factory<AssetModule>]
    delegate ValueTask<IAssetModule> Factory(string id, object? profile, IKeyLoader<AssetId, object> loader);

    string Id { get; }
    object? Profile { get; }
}

public static class AssetModuleExtensions
{
    public static async ValueTask<bool> Activate(this IAssetModule module, string id)
    {
        if ((await module.Claim<AssetId, object, IAssetTrigger>(new AssetId(module.Id, id))).TryGetValue(out IDisposable<IAssetTrigger>? trigger))
        {
            using (trigger)
            {
                await trigger.Value.Activate();
            }

            return true;
        }

        return false;
    }
}

public class AssetModule(string id, object? profile, IKeyLoader<AssetId, object> loader, [Inject<AssetCachePolicyTag>] IClaimCachePolicy<AssetId, object> cachePolicy) : ExtendedDisposable, IAssetModule
{
    private readonly ClaimCache<AssetId, object> cache = new(cachePolicy, loader);

    public string Id => id;
    public object? Profile => profile;

    public IAsyncEnumerable<KeyValuePair<AssetId, IDisposable<object>>> Claim(IEnumerable<AssetId> keys)
        => cache.Claim(keys);

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        cache.DisposeWith(this);
        loader.DisposeWith(this);
    }
}