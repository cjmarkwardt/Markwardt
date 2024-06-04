using System.Runtime.Loader;

namespace Markwardt;

[Singleton<AssetModuleStreamLoader>]
public interface IAssetModuleStreamLoader
{
    ValueTask<Failable<IAssetModule>> Load(IStreamPackage package);
}

public class AssetModuleStreamLoader : IAssetModuleStreamLoader
{
    public required IServiceResolver Resolver { get; init; }
    public required IAssetModule.Factory ModuleFactory { get; init; }

    public async ValueTask<Failable<IAssetModule>> Load(IStreamPackage package)
    {
        try
        {
            Loader loader = new(Resolver, package);
            return (await ModuleFactory(loader.ModuleId, await loader.GetModuleProfile(), loader)).AsFailable();
        }
        catch (Exception exception)
        {
            return exception;
        }
    }

    private sealed class Loader : AssetDataLoader
    {
        public Loader(IServiceResolver resolver, IStreamPackage package)
        {
            this.resolver = resolver;
            this.package = package;
            PluginContext context = new(package);
            Assembly assembly = context.LoadFromEntry("Plugin").NotNull("Asset plugin assembly not found");
            attribute = assembly.GetCustomAttribute<AssetModuleAttribute>().NotNull("Asset plugin assembly missing asset module attribute");
            loadAsset = AssetAttribute.CreateAssetFactory(resolver, assembly.GetExportedTypes());
        }

        private readonly IServiceResolver resolver;
        private readonly IStreamPackage package;
        private readonly AssetModuleAttribute attribute;
        private readonly Func<string, ValueTask<object?>> loadAsset;

        public string ModuleId => attribute.Id;

        public async ValueTask<object?> GetModuleProfile()
            => attribute.Profile is null ? null : await resolver.Create(attribute.Profile);

        protected override async ValueTask<object?> LoadAsset(string id)
            => await loadAsset(id);

        protected override ValueTask<IDisposable<Stream>?> LoadData(string id)
            => ValueTask.FromResult(package.Open($"Data/{id}")?.Buffer());
    }

    private sealed class PluginContext(IStreamPackage package) : AssemblyLoadContext(false)
    {
        public Assembly? LoadFromEntry(string name)
        {
            using IDisposable<Stream>? stream = package.Open($"Plugin/{name}.dll")?.Buffer();
            if (stream is not null)
            {
                return LoadFromStream(stream.Value);
            }

            return null;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
            => assemblyName.Name is not null ? LoadFromEntry(assemblyName.Name) : null;
    }
}