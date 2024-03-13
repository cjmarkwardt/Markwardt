namespace Markwardt;

public interface IPluginModule : IComplexDisposable
{
    [Factory<PluginModule>]
    delegate ValueTask<IPluginModule> Factory(string id, string name, string author, string description, IDynamicAssembly assembly);

    bool IsLoaded { get; }

    string Id { get; }
    string Name { get; }
    string Author { get; }
    string Description { get; }

    IObservableReadOnlyCache<string, IPlugin> Plugins { get; }

    ValueTask<bool> Load();
    ValueTask Reload();
    ValueTask<bool> Unload();
}

public class PluginModule(string id, string name, string author, string description, IDynamicAssembly assembly, IServiceResolver resolver) : ComplexDisposable, IPluginModule
{
    private readonly SequentialExecutor executor = new();

    public bool IsLoaded => assembly.IsLoaded;

    public string Id { get; } = id;
    public string Name { get; } = name;
    public string Author { get; } = author;
    public string Description { get; } = description;

    private readonly ObservableCache<string, IPlugin> plugins = new(x => x.Id, ItemDisposal.Full);
    public IObservableReadOnlyCache<string, IPlugin> Plugins => plugins;

    public async ValueTask<bool> Load()
        => await executor.Execute(async () =>
        {
            if (await assembly.Load())
            {
                await RefreshPlugins();
                return true;
            }

            return false;
        });

    public async ValueTask Reload()
        => await executor.Execute(async () =>
        {
            await assembly.Reload();
            await RefreshPlugins();
        });

    public async ValueTask<bool> Unload()
        => await executor.Execute(async () =>
        {
            if (await assembly.Unload())
            {
                await RefreshPlugins();
                return true;
            }

            return false;
        });

    protected override void PrepareDisposal()
    {
        base.PrepareDisposal();

        executor.DisposeWith(this);
        assembly.DisposeWith(this);
        plugins.DisposeWith(this);
    }

    private async ValueTask RefreshPlugins()
    {
        plugins.Clear();

        if (assembly.IsLoaded)
        {
            foreach (Type type in assembly.Value.GetTypes().Where(x => x.HasCustomAttribute<PluginAttribute>()))
            {
                plugins.Add((IPlugin)await resolver.Require(type));
            }
        }
    }
}