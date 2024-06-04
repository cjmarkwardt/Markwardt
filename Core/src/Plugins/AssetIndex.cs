namespace Markwardt;

[RoutedService<IAssetManager>]
public interface IAssetIndex
{
    IEnumerable<AssetModuleInfo> Modules { get; }
}