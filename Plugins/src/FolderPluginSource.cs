namespace Markwardt;

[Factory<FolderPluginSource>]
public delegate ValueTask<IPluginSource> FolderPluginSourceFactory(IFolder folder, IEnumerable<Type> sharedTypes);

public class FolderPluginSource(IFolder folder, IEnumerable<Type> sharedTypes) : ExtendedDisposable, IPluginSource
{
    private readonly SequentialExecutor executor = new();

    private readonly ObservableLookupList<string, IPluginModule> modules = new(x => x.Id) { ItemDisposal = ItemDisposal.Full };
    public IObservableReadOnlyLookupList<string, IPluginModule> Modules => modules;

    public required IPluginModuleReader ModuleReader { get; init; }

    public async ValueTask Refresh(bool purge = true)
        => await executor.Execute(async () =>
        {
            Failable<IEnumerable<IFolder>> tryDescend = (await folder.DescendAllFolders().Consolidate()).WithLogging(this, "Failed to refresh modules");
            if (tryDescend.IsFailure())
            {
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
            if (!modules.Lookup.ContainsKey(moduleFolder.Name))
            {
                Failable<IPluginModule> tryRead = (await ModuleReader.Read(moduleFolder.Name, moduleFolder, sharedTypes)).WithLogging(this, $"Failed to read module at {moduleFolder.FullName}");
                if (tryRead.IsSuccess())
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
            modules.Remove(purgedModule.Id);
            await purgedModule.DisposeAsync();
        }
    }
}