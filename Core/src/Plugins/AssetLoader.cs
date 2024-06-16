namespace Markwardt;

[ServiceType<IKeyLoader<AssetId, object>>]
public class AssetLoaderTag : ImplementationTag<AssetLoader>;

public class AssetLoader(IAssetReader reader) : ExtendedDisposable, IKeyLoader<AssetId, object>
{
    public AssetLoader(IServiceResolver resolver, IDataAccessor<string, Stream?> package)
    {
        this.resolver = resolver;
        this.package = package;
        PluginContext context = new(package);
        Assembly assembly = context.LoadFromEntry("Plugin").NotNull("Asset plugin assembly not found");
        attribute = assembly.GetCustomAttribute<AssetModuleAttribute>().NotNull("Asset plugin assembly missing asset module attribute");
        loadAsset = AssetAttribute.CreateAssetFactory(resolver, assembly.GetExportedTypes());
    }

    private readonly IServiceResolver resolver;
    private readonly IDataAccessor<string, Stream?> package;
    private readonly AssetModuleAttribute attribute;
    private readonly Func<string, ValueTask<object?>> loadAsset;

    public string ModuleId => attribute.Id;

    public async ValueTask<object?> GetModuleProfile()
        => attribute.Profile is null ? null : await resolver.Create(attribute.Profile);

    protected override async ValueTask<object?> LoadAsset(AssetId id)
        => await loadAsset(id.Value);

    protected override ValueTask<IDisposable<Stream>?> LoadData(AssetId id)
        => ValueTask.FromResult(package.Open($"Data/{id.Value}")?.Buffer());

    public IAsyncEnumerable<KeyValuePair<AssetId, object>> Load(IEnumerable<AssetId> keys)
        => keys.Select(x => Load(x).AsTask()).AwaitMerge().WhereValueNotNull();

    protected virtual ValueTask<object?> LoadAsset(AssetId id) => ValueTask.FromResult<object?>(null);
    protected virtual ValueTask<IDisposable<Stream>?> LoadData(AssetId id) => ValueTask.FromResult<IDisposable<Stream>?>(null);

    private async ValueTask<KeyValuePair<AssetId, object>?> Load(AssetId id)
    {
        object? asset = await LoadAsset(id);

        if (asset is null && reader is not null)
        {
            await using IDisposable<Stream>? stream = await LoadData(id);
            if (stream is not null)
            {
                asset = await reader.Read(id, stream.Value);
            }
        }

        return asset is null ? null : new(id, asset);
    }
}