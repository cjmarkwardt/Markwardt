namespace Markwardt;

[Singleton<ModuleReader>]
public interface IModuleReader
{
    ValueTask<Failable<IModule>> Read(string id, IFolder folder, IEnumerable<Type> sharedTypes);
}

public class ModuleReader : IModuleReader
{
    public required IModule.Factory ModuleFactory { get; init; }
    public required IDynamicAssembly.Factory AssemblyFactory { get; init; }

    public async ValueTask<Failable<IModule>> Read(string id, IFolder folder, IEnumerable<Type> sharedTypes)
    {
        Failable<string> tryReadProfile = await folder.Descend("Profile.json").AsFile().ReadText();
        if (tryReadProfile.Exception != null)
        {
            return tryReadProfile.Exception.AsFailable<IModule>("Failed to read profile file");
        }

        JsonObject? profile = JsonNode.Parse(tryReadProfile.Result) as JsonObject;
        if (profile == null)
        {
            return Failable.Fail<IModule>("Profile file is invalid");
        }

        string name = profile["Name"]?.ToString() ?? id;
        string author = profile["Author"]?.ToString() ?? string.Empty;
        string description = profile["Description"]?.ToString() ?? string.Empty;

        return Failable.Success(await ModuleFactory(id, name, author, description, await AssemblyFactory(folder.Descend("Assembly.dll").AsFile(), sharedTypes)));
    }
}