namespace Markwardt;

[RoutedService<IAssetManager>]
public interface IAssetSource : IClaimSource<AssetId, object>
{
    AssetId GetId(object asset);
}