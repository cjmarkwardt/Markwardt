namespace Markwardt;

[Singleton<AssetManager>]
public interface IAssetManager : IAssetIndex, IAssetSource, IAssetActivator, IMultiDisposable
{
    void Add(IAssetModule module);
    bool Remove(string module);
    bool Contains(string module);
    bool Clear();
}

public class AssetManager : ExtendedDisposable, IAssetManager
{
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

    public async ValueTask<Maybe<IDisposable<object>>> Load(AssetId id)
    {
        Maybe<IDisposable<object>> asset = await modules[id.Module].Claim(id);
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

    public IAsyncEnumerable<KeyValuePair<AssetId, IDisposable<object>>> Claim(IEnumerable<AssetId> keys)
        => keys.GroupBy(x => x.Module).Where(x => modules.ContainsKey(x.Key)).Select(x => modules[x.Key].Claim(x)).Merge();
}