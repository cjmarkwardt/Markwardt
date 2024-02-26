namespace Markwardt.Godot;

public class TextAssetLoaderTag : ImplementationTag<TextAssetLoader> { }

public class TextAssetLoader : Loader
{
    public override ValueTask<Failable<object>> TryLoad(string path, Type? type = null)
    {
        using Godot.FileAccess file = Godot.FileAccess.Open(path, Godot.FileAccess.ModeFlags.Read);
        return Failable.Success<object>(file.GetAsText());
    }
}