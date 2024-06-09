namespace Markwardt;

public interface IAssetModule : IMultiDisposable
{
    [Factory<AssetModule>]
    delegate ValueTask<IAssetModule> Factory(string id, object? profile, IAssetDataLoader loader);

    string Id { get; }
    object? Profile { get; }

    ValueTask<Maybe<IDisposable<object>>> Load(string id, IAssetDataReader? reader = null);
}

public static class AssetModuleExtensions
{
    public static async ValueTask<Maybe<IDisposable<T>>> Load<T>(this IAssetModule module, string id, IClaimCachePolicy<AssetId, T>? cachePolicy = null, IAssetDataReader? reader = null)
        => (await module.Load(id, cachePolicy, reader)).Select(x => x.Cast<T>());

    public static async ValueTask<bool> Activate(this IAssetModule module, string id)
    {
        if ((await module.Load<IAssetTrigger>(id)).TryGetValue(out IDisposable<IAssetTrigger>? trigger))
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

public class AssetModule(string id, object? profile, IAssetDataLoader loader, [Inject<AssetCachePolicyTag>] IClaimCachePolicy<AssetId, object> cachePolicy) : ExtendedDisposable, IAssetModule
{
    private static ClaimCachePolicy<string, object> GetCachePolicy(string module, IClaimCachePolicy<AssetId, object> basePolicy)
        => new((id, asset, claims, lastClaim, lastRelease) => basePolicy.IsExpired(new(module, id), asset, claims, lastClaim, lastRelease));

    private static CacheKeyLoader<string, object> GetKeyLoader(IAssetDataLoader loader)
        => new(async ids => await Task.WhenAll(ids.Select(x => )));

    private readonly ClaimCache<string, object> cache = new(GetCachePolicy(id, cachePolicy), GetKeyLoader(loader));

    public string Id => id;
    public object? Profile => profile;

    public async ValueTask<Maybe<IDisposable<object>>> Load(string id, IAssetDataReader? reader = null)
    {
        if (cache.Claim(id).TryGetValue(out IDisposable<object>? claim))
        {
            return claim.Maybe();
        }
        else
        {
            object? asset = await loader.Load(id, reader);
            if (asset is not null)
            {
                cache.Set(id, asset, cachePolicy);
                return cache.Claim(id).Value.Maybe();
            }
            else
            {
                return default;
            }
        }
    }

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        cache.DisposeWith(this);
        loader.DisposeWith(this);
    }

    private sealed record ClaimRequest(string Id, IAssetDataReader)
}