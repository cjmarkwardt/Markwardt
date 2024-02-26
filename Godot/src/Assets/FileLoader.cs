namespace Markwardt.Godot;

public class FileLoaderTag : ImplementationTag<FileLoader> { }

public class FileLoader([Inject<GltfLoaderTag>] ILoader gltfLoader, [Inject<ImageLoaderTag>] ILoader imageLoader) : Loader
{
    public override async ValueTask<Failable<object>> TryLoad(string path, Type? type = null)
    {
        if (path.EndsWith(".glb") || path.EndsWith(".gltf"))
        {
            return await gltfLoader.Load(path, type);
        }
        else if (path.EndsWith(".png") || path.EndsWith(".jpg") || path.EndsWith(".jpeg") || path.EndsWith(".bmp") || path.EndsWith(".dds") || path.EndsWith(".exr") || path.EndsWith(".hdr") || path.EndsWith(".tga") || path.EndsWith(".svg") || path.EndsWith(".svgz") || path.EndsWith(".webp"))
        {
            return await imageLoader.Load(path, type);
        }
        else if (path.EndsWith(".script"))
        {
            return await File.ReadAllTextAsync(path.AsLocalPath());
        }
        else
        {
            throw new InvalidOperationException($"Unrecognized format for path {path}");
        }
    }
}