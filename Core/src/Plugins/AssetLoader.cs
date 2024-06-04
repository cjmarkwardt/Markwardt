namespace Markwardt;

[RoutedService<IAssetManager>]
public interface IAssetLoader
{
    ValueTask<Maybe<IDisposable<object>>> Load(AssetId id, IClaimCachePolicy<object>? cachePolicy = null, IAssetDataReader? reader = null);
    AssetId GetId(object asset);
}

public static class AssetLoaderExtensions
{
    public static async ValueTask<Maybe<IDisposable<T>>> Load<T>(this IAssetLoader loader, AssetId id, IClaimCachePolicy<object>? cachePolicy = null, IAssetDataReader? reader = null)
        => (await loader.Load(id, cachePolicy, reader)).Select(x => x.Cast<T>());
}