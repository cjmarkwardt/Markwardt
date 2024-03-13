namespace Markwardt;

[Factory<FolderPluginSource>]
public delegate ValueTask<IPluginSource> FolderPluginSourceFactory(IFolder folder, IEnumerable<Type> sharedTypes);

public class FolderPluginSource(IFolder folder, IEnumerable<Type> sharedTypes) : ComplexDisposable, IPluginSource
{
    private readonly SequentialExecutor executor = new();

    private readonly ObservableCache<string, IPluginModule> modules = new(x => x.Id, ItemDisposal.Full);
    public IObservableReadOnlyCache<string, IPluginModule> Modules => modules;

    public required IPluginModuleReader ModuleReader { get; init; }

    public async ValueTask Refresh(bool purge = true)
        => await executor.Execute(async () =>
        {
            Failable<IEnumerable<IFolder>> tryDescend = await folder.DescendAllFolders().Consolidate();
            if (tryDescend.Exception != null)
            {
                Fail(tryDescend.Exception.AsFailable("Failed to refresh modules"));
                return;
            }

            await Read(tryDescend.Result);

            if (purge)
            {
                await Purge(tryDescend.Result);
            }
        });

    private async ValueTask Read(IEnumerable<IFolder> moduleFolders)
    {
        foreach (IFolder moduleFolder in moduleFolders)
        {
            if (!modules.ContainsKey(moduleFolder.Name))
            {
                Failable<IPluginModule> tryRead = await ModuleReader.Read(moduleFolder.Name, moduleFolder, sharedTypes);
                if (tryRead.Exception != null)
                {
                    Fail(tryRead.Exception.AsFailable($"Failed to read module at {moduleFolder.FullName}"));
                }
                else
                {
                    modules.Add(tryRead.Result);
                }
            }
        }
    }

    private async ValueTask Purge(IEnumerable<IFolder> moduleFolders)
    {
        HashSet<string> existingModules = moduleFolders.Select(x => x.Name).ToHashSet();
        
        foreach (IPluginModule purgedModule in modules.Where(x => !existingModules.Contains(x.Id)))
        {
            modules.RemoveKey(purgedModule.Id);
            await purgedModule.DisposeAsync();
        }
    }
}