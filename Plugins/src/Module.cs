namespace Markwardt;

public interface IModule : IComponent
{
    [Factory<Module>]
    delegate ValueTask<IModule> Factory(string id, string name, string author, string description, IDataStore data, IDynamicAssembly assembly, IMultiStreamSource assets);

    bool IsLoaded { get; }

    string Id { get; }
    string Name { get; }
    string Author { get; }
    string Description { get; }

    IDataStore Data { get; }
    IReadOnlyDictionary<string, IPlugin> Plugins { get; }

    IStreamSource GetAsset(string id);

    bool Load();
    void Reload();
    bool Unload();
}

public static class ModuleExtensions
{
    public static Option<T> GetPlugin<T>(this IModule module, string id)
        where T : IPlugin
        => module.Plugins.TryGetValue(id, out IPlugin? plugin) && plugin is T casted ? casted.Some() : Option.None<T>();
}

public class Module : Component, IModule
{
    public Module(string id, string name, string author, string description, IDataStore data, IDynamicAssembly assembly, IMultiStreamSource assets)
    {
        Id = id;
        Name = name;
        Author = author;
        Description = description;
        Data = data;

        this.assembly = assembly.DisposeWith(this);
        this.assets = assets;
    }

    private readonly IDynamicAssembly assembly;
    private readonly IMultiStreamSource assets;
    private readonly Dictionary<string, IPlugin> plugins = [];

    public bool IsLoaded => assembly.IsLoaded;

    public string Id { get; }
    public string Name { get; }
    public string Author { get; }
    public string Description { get; }

    public IDataStore Data { get; }
    public IReadOnlyDictionary<string, IPlugin> Plugins => plugins;

    public IStreamSource GetAsset(string id)
        => assets.Get(id);

    public bool Load()
    {
        Verify();

        if (assembly.Load())
        {
            RefreshPlugins();
            return true;
        }

        return false;
    }

    public void Reload()
    {
        Verify();
        assembly.Reload();
        RefreshPlugins();
    }

    public bool Unload()
    {
        if (assembly.Unload())
        {
            RefreshPlugins();
            return true;
        }

        return false;
    }

    protected override void OnDisposal()
    {
        base.OnDisposal();

        Unload();
    }

    private void RefreshPlugins()
    {
        plugins.Clear();

        if (assembly.IsLoaded)
        {
            foreach (Type type in assembly.Value.GetTypes().Where(x => x.IsClass && !x.IsAbstract && x.GetCustomAttribute<PluginAttribute>() != null && x.IsAssignableTo(typeof(IPlugin))))
            {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(type)!;
                plugins.Add(plugin.Id, plugin);
            }
        }
    }
}