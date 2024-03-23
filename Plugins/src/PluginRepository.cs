namespace Markwardt;

public interface IPluginRepository : IComplexDisposable
{
    IObservableReadOnlyCache<string, IPluginModule> Modules { get; }

    ValueTask Refresh(bool purge = true);
}

public static class PluginRepositoryExtensions
{
    public static async ValueTask LoadAll(this IPluginRepository plugins)
        => await Task.WhenAll(plugins.Modules.Select(async x => await x.Load()));

    public static async ValueTask ReloadAll(this IPluginRepository plugins)
        => await Task.WhenAll(plugins.Modules.Where(x => x.IsLoaded).Select(async x => await x.Reload()));

    public static async ValueTask UnloadAll(this IPluginRepository plugins)
        => await Task.WhenAll(plugins.Modules.Select(async x => await x.Unload()));

    public static Maybe<IPlugin> GetPlugin(this IPluginRepository plugins, string moduleId, string id)
        => plugins.Modules.GetValue(moduleId).Select(x => x.Plugins.GetValue(id));
}

public class PluginRepository : ComplexDisposable, IPluginRepository
{
    public PluginRepository([Inject<PluginSourcesTag>] IObservableReadOnlyCollection<IPluginSource> sources)
    {
        this.sources = sources;

        Modules = sources.Observe().TransformMany(x => x.Modules).ToCache(x => x.Id).DisposeWith(this);
    }

    private readonly IObservableReadOnlyCollection<IPluginSource> sources;

    public IObservableReadOnlyCache<string, IPluginModule> Modules { get; }

    public async ValueTask Refresh(bool purge = true)
        => await Task.WhenAll(sources.Select(x => x.Refresh(purge).AsTask()));
}