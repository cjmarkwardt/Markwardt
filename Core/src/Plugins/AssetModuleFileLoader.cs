namespace Markwardt;

[RoutedService<AssetModuleZipLoaderTag>]
public interface IAssetModuleFileLoader
{
    ValueTask<Failable<IAssetModule>> Load(string path);
}