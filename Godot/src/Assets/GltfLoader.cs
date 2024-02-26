namespace Markwardt.Godot;

public class GltfLoaderTag : ImplementationTag<GltfLoader> { }

public class GltfLoader : CachedLoader
{
    private readonly GltfDocument document = new();

    protected override async ValueTask<Failable<object>> AttemptLoad(string path, Type? type = null)
    {
        GltfState state = new();

        using (MemoryStream data = new())
        {
            using (FileStream file = File.OpenRead(path.AsLocalPath()))
            {
                await file.CopyToAsync(data);
            }

            document.AppendFromBuffer(data.ToArray(), string.Empty, state);
        }

        Node3D node = (Node3D)document.GenerateScene(state);
        node.Name = Path.GetFileNameWithoutExtension(path);

        ModelSource model = new(node);
        SetCache(path, type, model);
        return model;
    }
}