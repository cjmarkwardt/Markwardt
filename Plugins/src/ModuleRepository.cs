namespace Markwardt;

public interface IModuleRepository : IComponent
{
    [Factory<ModuleRepository>]
    delegate ValueTask<IModuleRepository> Factory(IFolder folder, IEnumerable<Type> sharedTypes);

    IReadOnlyDictionary<string, IModule> Modules { get; }

    ValueTask Refresh(bool purge = true);
}

public static class ModuleRepositoryExtensions
{
    public static void LoadAll(this IModuleRepository plugins)
    {
        foreach (IModule module in plugins.Modules.Values)
        {
            module.Load();
        }
    }

    public static void ReloadAll(this IModuleRepository plugins)
    {
        foreach (IModule module in plugins.Modules.Values.Where(x => x.IsLoaded))
        {
            module.Reload();
        }
    }

    public static void UnloadAll(this IModuleRepository plugins)
    {
        foreach (IModule module in plugins.Modules.Values)
        {
            module.Unload();
        }
    }

    public static Option<IPlugin> GetPlugin(this IModuleRepository plugins, string moduleId, string id)
        => plugins.Modules.TryGetValue(moduleId, out IModule? module) ? module.Plugins.GetValue(id) : Option.None<IPlugin>();

    public static Option<T> GetPlugin<T>(this IModuleRepository plugins, string moduleId, string id)
        where T : IPlugin
        => plugins.Modules.TryGetValue(moduleId, out IModule? module) ? module.GetPlugin<T>(id) : Option.None<T>();

    public static Option<IDataStore> GetData(this IModuleRepository plugins, string moduleId)
        => plugins.Modules.TryGetValue(moduleId, out IModule? module) ? module.Data.Some() : Option.None<IDataStore>();

    public static Option<IStreamSource> GetAsset(this IModuleRepository plugins, string moduleId, string id)
        => plugins.Modules.TryGetValue(moduleId, out IModule? module) ? module.GetAsset(id).Some() : Option.None<IStreamSource>();
}

public class ModuleRepository(IFolder folder, IEnumerable<Type> sharedTypes) : Component, IModuleRepository
{
    private readonly SequentialExecutor executor = new();
    private readonly Dictionary<string, IModule> modules = [];

    public required IModuleReader ModuleReader { get; init; }

    public IReadOnlyDictionary<string, IModule> Modules => modules;

    public async ValueTask Refresh(bool purge = true)
        => await executor.Execute(async () =>
        {
            Failable<IEnumerable<IFolder>> tryDescend = await folder.DescendAllFolders().Consolidate();
            if (tryDescend.Exception != null)
            {
                this.Error(tryDescend.Exception.AsFailable("Failed to refresh modules"));
                return;
            }

            IEnumerable<IFolder> subFolders = tryDescend.Result;

            foreach (IFolder subFolder in subFolders)
            {
                if (!modules.ContainsKey(subFolder.Name))
                {
                    Failable<IModule> tryRead = await ModuleReader.Read(subFolder.Name, subFolder, sharedTypes);
                    if (tryRead.Exception != null)
                    {
                        this.Error(tryRead.Exception.AsFailable($"Failed to read module at {subFolder.FullName}"));
                    }

                    modules.Add(subFolder.Name, tryRead.Result);
                }
            }

            if (purge)
            {
                IEnumerable<string> existingModules = subFolders.Select(x => x.Name).ToHashSet();
                
                foreach (IModule purgedModule in modules.Values.Where(x => !existingModules.Contains(x.Id)))
                {
                    modules.Remove(purgedModule.Id);
                    await purgedModule.DisposeAsync();
                }
            }
        });

    protected override void PrepareDisposal()
    {
        base.PrepareDisposal();

        executor.DisposeWith(this);
    }
}