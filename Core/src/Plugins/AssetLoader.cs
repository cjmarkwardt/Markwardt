namespace Markwardt;

[RoutedService<IAssetManager>]
public interface IAssetLoader
{
    ValueTask<Maybe<IDisposable<object>>> Load(AssetId id, IAssetDataReader? reader = null);
    AssetId GetId(object asset);
}

public static class AssetLoaderExtensions
{
    public static async ValueTask<Maybe<IDisposable<T>>> Load<T>(this IAssetLoader loader, AssetId id, IAssetDataReader? reader = null)
        => (await loader.Load(id, reader)).Select(x => x.Cast<T>());
}