namespace Markwardt;

[Singleton<PluginModuleReader>]
public interface IPluginModuleReader
{
    ValueTask<Failable<IPluginModule>> Read(string id, IFolder folder, IEnumerable<Type> sharedTypes);
}

public class PluginModuleReader : IPluginModuleReader
{
    public required IPluginModule.Factory ModuleFactory { get; init; }
    public required IDynamicAssembly.Factory AssemblyFactory { get; init; }

    public async ValueTask<Failable<IPluginModule>> Read(string id, IFolder folder, IEnumerable<Type> sharedTypes)
    {
        Failable<string> tryReadProfile = await folder.Descend("Profile.json").AsFile().ReadText();
        if (tryReadProfile.Exception != null)
        {
            return tryReadProfile.Exception.AsFailable<IPluginModule>("Failed to read profile file");
        }

        JsonObject? profile = JsonNode.Parse(tryReadProfile.Result) as JsonObject;
        if (profile == null)
        {
            return Failable.Fail<IPluginModule>("Profile file is invalid");
        }

        string name = profile["Name"]?.ToString() ?? id;
        string author = profile["Author"]?.ToString() ?? string.Empty;
        string description = profile["Description"]?.ToString() ?? string.Empty;

        return Failable.Success(await ModuleFactory(id, name, author, description, await AssemblyFactory(folder.Descend("Assembly.dll").AsFile(), sharedTypes)));
    }
}