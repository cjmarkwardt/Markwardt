namespace Markwardt;

[Singleton<AssetManager>]
public interface IAssetManager : IAssetIndex, IAssetLoader, IAssetActivator, IMultiDisposable
{
    void Add(IAssetModule module);
    bool Remove(string module);
    bool Contains(string module);
    bool Clear();

    IDisposable ConfigureReader(Func<AssetId, bool> isValid, IAssetDataReader reader);
}

public class AssetManager : ExtendedDisposable, IAssetManager
{
    private readonly HashSet<Configuration<IAssetDataReader>> readerConfigurations = [];
    private readonly ConditionalWeakTable<object, AssetId> ids = [];

    private readonly Dictionary<string, IAssetModule> modules = [];
    public IEnumerable<AssetModuleInfo> Modules => modules.Values.Select(x => new AssetModuleInfo(x.Id, x.Profile));

    public void Add(IAssetModule module)
        => modules.Add(module.Id, module);

    public bool Remove(string module)
    {
        if (modules.TryGetValue(module, out IAssetModule? instance))
        {
            modules.Remove(module);
            this.DisposeInBackground(instance);
            return true;
        }

        return false;
    }

    public bool Contains(string module)
        => modules.ContainsKey(module);

    public bool Clear()
    {
        if (modules.Count > 0)
        {
            modules.Clear();
            return true;
        }

        return false;
    }

    public async ValueTask<Maybe<IDisposable<object>>> Load(AssetId id, IAssetDataReader? reader = null)
    {
        Maybe<IDisposable<object>> asset = await modules[id.Module].Load(id.Value, reader ?? GetConfiguration(readerConfigurations, id));
        if (asset.HasValue)
        {
            ids.AddOrUpdate(asset.Value.Value, id);
        }

        return asset;
    }

    public AssetId GetId(object asset)
        => ids.GetOrCreateValue(asset);

    public async ValueTask Activate(string id)
        => await Task.WhenAll(modules.Values.Select(x => x.Activate(id).AsTask()));

    public IDisposable ConfigureReader(Func<AssetId, bool> isValid, IAssetDataReader reader)
        => Configure(readerConfigurations, isValid, reader);

    private IDisposable Configure<T>(ICollection<Configuration<T>> configurations, Func<AssetId, bool> isValid, T value)
        where T : class
    {
        Configuration<T> configuration = new(isValid, value);
        configurations.Add(configuration);
        return Disposable.Create(() => configurations.Remove(configuration));
    }

    private T? GetConfiguration<T>(IEnumerable<Configuration<T>> configurations, AssetId id)
        where T : class
        => configurations.FirstOrDefault(x => x.Get(id) is not null)?.Get(id);

    private sealed class Configuration<T>(Func<AssetId, bool> isValid, T value)
        where T : class
    {
        public T? Get(AssetId id)
            => isValid(id) ? value : null;
    }
}