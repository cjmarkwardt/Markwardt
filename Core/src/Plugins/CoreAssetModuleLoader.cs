namespace Markwardt;

[Singleton<CoreAssetModuleLoader>]
public interface ICoreAssetModuleLoader
{
    ValueTask<IAssetModule> Load(string id, object? profile = null, Func<string, ValueTask<object?>>? loadAsset = null, Func<string, ValueTask<IDisposable<Stream>?>>? loadData = null);
}

public class CoreAssetModuleLoader : ICoreAssetModuleLoader
{
    public required IServiceResolver Resolver { get; init; }
    public required IAssetModule.Factory ModuleFactory { get; init; }

    public async ValueTask<IAssetModule> Load(string id, object? profile = null, Func<string, ValueTask<object?>>? loadAsset = null, Func<string, ValueTask<IDisposable<Stream>?>>? loadData = null)
        => await ModuleFactory(id, profile, new Loader(Resolver, loadAsset ?? (_ => ValueTask.FromResult<object?>(null)), loadData ?? (_ => ValueTask.FromResult<IDisposable<Stream>?>(null))));

    private sealed class Loader(IServiceResolver resolver, Func<string, ValueTask<object?>> loadAsset, Func<string, ValueTask<IDisposable<Stream>?>> loadData) : AssetDataLoader
    {
        private readonly Func<string, ValueTask<object?>> loadAssemblyAsset = AssetAttribute.CreateAssetFactory(resolver, AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetExportedTypes()));

        protected override async ValueTask<object?> LoadAsset(string id)
            => await loadAssemblyAsset(id) ?? await loadAsset(id);

        protected override async ValueTask<IDisposable<Stream>?> LoadData(string id)
            => await loadData(id);
    }
}