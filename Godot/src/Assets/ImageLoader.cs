namespace Markwardt.Godot;

public class ImageLoaderTag : ImplementationTag<ImageLoader> { }

public class ImageLoader : Loader
{
    public override ValueTask<Failable<object>> TryLoad(string path, Type? type = null)
        => Failable.Success<object>(ImageTexture.CreateFromImage(Image.LoadFromFile(path.AsLocalPath())));
}