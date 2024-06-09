namespace Markwardt;

[ServiceType<IClaimCachePolicy<AssetId, object>>]
public class AssetCachePolicyTag : DelegateTag
{
    protected override ValueTask<object> Create(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments)
        => ValueTask.FromResult<object>(new ClaimCachePolicy<AssetId, object>((_, _, claims, _, lastRelease) => claims == 0 && lastRelease?.AddMinutes(5) > DateTime.Now));
}