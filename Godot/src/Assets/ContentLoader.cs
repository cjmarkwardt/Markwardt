namespace Markwardt.Godot;

public class ContentLoaderTag : ImplementationTag<ContentLoader> { }

public class ContentLoader([Inject<AssetLoaderTag>] ILoader assets, [Inject<FileLoaderTag>] ILoader files, [Inject<GameModuleFolderTag>] string moduleFolder) : CachedLoader
{
    protected override async ValueTask<Failable<object>> AttemptLoad(string path, Type? type = null)
    {
        if (path.StartsWith("core/"))
        {
            return await assets.Load("assets/" + path, type);
        }
        else
        {
            return await files.Load(moduleFolder + "/" + path, type);
        }
    }
}