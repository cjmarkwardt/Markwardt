namespace Markwardt;

public interface IPluginRepository : IComponent, IEnumerable<IPlugin>
{
    [Factory<PluginRepository>]
    delegate ValueTask<IPluginRepository> Factory(IFolder folder, IEnumerable<Type> sharedTypes);

    IEnumerable<IModule> Modules { get; }

    ValueTask Refresh(bool purge = true);

    Option<IModule> GetModule(string id);
    Option<IPlugin> GetPlugin(string moduleId, string id);
}

public static class PluginRepositoryExtensions
{
    public static void LoadAll(this IPluginRepository plugins)
    {
        foreach (IModule module in plugins.Modules)
        {
            module.Load();
        }
    }

    public static void ReloadAll(this IPluginRepository plugins)
    {
        foreach (IModule module in plugins.Modules.Where(x => x.IsLoaded))
        {
            module.Reload();
        }
    }

    public static void UnloadAll(this IPluginRepository plugins)
    {
        foreach (IModule module in plugins.Modules)
        {
            module.Unload();
        }
    }
}

public class PluginRepository(IFolder folder, IEnumerable<Type> sharedTypes) : Component, IPluginRepository
{
    private readonly SequentialExecutor executor = new();
    private readonly Dictionary<string, IModule> modules = [];

    public required IModuleReader ModuleReader { get; init; }

    public IEnumerable<IModule> Modules => modules.Values;

    public ValueTask Refresh(bool purge = true)
        => executor.Execute(async () =>
        {
            Failable<IEnumerable<IFolder>> tryDescend = await folder.DescendAllFolders().Consolidate();
            if (tryDescend.Exception != null)
            {
                Error(tryDescend.Exception.AsFailable("Failed to refresh modules"));
                return;
            }

            IEnumerable<IFolder> folders = tryDescend.Result;

            foreach (IFolder folder in folders)
            {
                if (!modules.ContainsKey(folder.Name))
                {
                    Failable<IModule> tryRead = await ModuleReader.Read(folder.Name, folder, sharedTypes);
                    if (tryRead.Exception != null)
                    {
                        Error(tryRead.Exception.AsFailable($"Failed to read module at {folder.FullName}"));
                    }

                    modules.Add(folder.Name, tryRead.Result);
                }
            }

            if (purge)
            {
                IEnumerable<string> existingModules = folders.Select(x => x.Name).ToHashSet();
                
                foreach (IModule purgedModule in modules.Values.Where(x => !existingModules.Contains(x.Id)))
                {
                    modules.Remove(purgedModule.Id);
                    await purgedModule.DisposeAsync();
                }
            }
        });

    public Option<IModule> GetModule(string id)
        => modules.TryGetValue(id, out IModule? module) ? module.Some() : Option.None<IModule>();

    public Option<IPlugin> GetPlugin(string moduleId, string id)
        => GetModule(moduleId).TryGetValue(out IModule? module) ? module.GetPlugin(id) : Option.None<IPlugin>();

    public IEnumerator<IPlugin> GetEnumerator()
        => modules.Values.SelectMany(x => x).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

    protected override void PrepareDisposal()
    {
        base.PrepareDisposal();

        executor.DisposeWith(this);
    }
}