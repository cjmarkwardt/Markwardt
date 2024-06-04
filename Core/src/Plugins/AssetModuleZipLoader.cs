using System.IO.Compression;

namespace Markwardt;

[ServiceType<IAssetModuleFileLoader>]
public class AssetModuleZipLoaderTag : ImplementationTag<AssetModuleZipLoader>;

public class AssetModuleZipLoader : IAssetModuleFileLoader
{
    public required IAssetModuleStreamLoader ModuleLoader { get; init; }
    public required ZipStreamPackageFactory ZipPackageFactory { get; init; }

    public async ValueTask<Failable<IAssetModule>> Load(string path)
        => await ModuleLoader.Load(await ZipPackageFactory(() => ZipFile.OpenRead(path)));
}