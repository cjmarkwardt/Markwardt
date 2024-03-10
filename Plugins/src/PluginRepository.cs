namespace Markwardt;

public interface IPluginRepository : IComplexDisposable, IEnumerable<IPlugin>
{
    [Factory<PluginRepository>]
    delegate ValueTask<IPluginRepository> Factory(IFolder folder, IEnumerable<Type> sharedTypes);

    IEnumerable<IModule> Modules { get; }

    ValueTask Refresh(bool purge = true);

    IMaybe<IModule> GetModule(string id);
    IMaybe<IPlugin> GetPlugin(string moduleId, string id);
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

public class PluginRepository(IFolder folder, IEnumerable<Type> sharedTypes) : ComplexDisposable, IPluginRepository
{
    private readonly SequentialExecutor executor = new();
    private readonly Dictionary<string, IModule> modules = [];

    public required IModuleReader ModuleReader { get; init; }

    public IEnumerable<IModule> Modules => modules.Values;

    public async ValueTask Refresh(bool purge = true)
        => await executor.Execute(async () =>
        {
            Failable<IEnumerable<IFolder>> tryDescend = await folder.DescendAllFolders().Consolidate();
            if (tryDescend.Exception != null)
            {
                this.LogError(tryDescend.Exception.AsFailable("Failed to refresh modules"));
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
                        this.LogError(tryRead.Exception.AsFailable($"Failed to read module at {subFolder.FullName}"));
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

    public IMaybe<IModule> GetModule(string id)
        => modules.TryGetValue(id, out IModule? module) ? module.AsMaybe() : Maybe<IModule>.Empty();

    public IMaybe<IPlugin> GetPlugin(string moduleId, string id)
        => GetModule(moduleId).TryGetValue(out IModule module) ? module.GetPlugin(id) : Maybe<IPlugin>.Empty();

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