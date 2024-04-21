namespace Markwardt;

public interface IPluginRepository : IExtendedDisposable
{
    IObservableReadOnlyLookupList<string, IPluginModule> Modules { get; }

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

public class PluginRepository : ExtendedDisposable, IPluginRepository
{
    public PluginRepository([Inject<PluginSourcesTag>] IObservableReadOnlyList<IPluginSource> sources)
    {
        this.sources = sources;

        Modules = sources.Connect().TransformMany(x => x.Modules.AsObservableList()).ObserveAsLookupList(x => x.Id).DisposeWith(this);
    }

    private readonly IObservableReadOnlyList<IPluginSource> sources;

    public IObservableReadOnlyLookupList<string, IPluginModule> Modules { get; }

    public async ValueTask Refresh(bool purge = true)
        => await Task.WhenAll(sources.Select(x => x.Refresh(purge).AsTask()));
}