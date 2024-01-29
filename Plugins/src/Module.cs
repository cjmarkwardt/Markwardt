namespace Markwardt;

public interface IModule : IComponent, IEnumerable<IPlugin>
{
    [Factory<Module>]
    delegate ValueTask<IModule> Factory(string id, string name, string author, string description, IDynamicAssembly assembly);

    bool IsLoaded { get; }

    string Id { get; }
    string Name { get; }
    string Author { get; }
    string Description { get; }

    bool Load();
    void Reload();
    bool Unload();

    Option<IPlugin> GetPlugin(string id);
}

public static class ModuleExtensions
{
    public static Option<T> GetPlugin<T>(this IModule module, string id)
        => module.GetPlugin(id).Map(x => (T)x);
}

public class Module : Component, IModule
{
    public Module(string id, string name, string author, string description, IDynamicAssembly assembly)
    {
        Id = id;
        Name = name;
        Author = author;
        Description = description;
        this.assembly = assembly.DisposeWith(this);
    }

    private readonly IDynamicAssembly assembly;
    private readonly Dictionary<string, IPlugin> plugins = [];

    public bool IsLoaded => assembly.IsLoaded;

    public string Id { get; }
    public string Name { get; }
    public string Author { get; }
    public string Description { get; }

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

    public Option<IPlugin> GetPlugin(string id)
        => plugins.TryGetValue(id, out IPlugin? plugin) ? plugin.Some() : Option.None<IPlugin>();

    public IEnumerator<IPlugin> GetEnumerator()
        => plugins.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

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