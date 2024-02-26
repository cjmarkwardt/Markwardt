namespace Markwardt.Godot;

public class AssetLoaderTag : ImplementationTag<AssetLoader> { }

public class AssetLoader : Loader
{
    public override async ValueTask<Failable<object>> TryLoad(string path, Type? type = null)
    {
        ResourceLoader.LoadThreadedRequest(path).Verify();

        while (true)
        {
            ResourceLoader.ThreadLoadStatus status = ResourceLoader.LoadThreadedGetStatus(path);
            if (status is ResourceLoader.ThreadLoadStatus.Loaded)
            {
                Resource resource = ResourceLoader.LoadThreadedGet(path);
                return resource is PackedScene scene ? scene.Instantiate() : resource;
            }
            else if (status is ResourceLoader.ThreadLoadStatus.InvalidResource)
            {
                throw new InvalidOperationException($"Invalid resource ({path})");
            }
            else if (status is ResourceLoader.ThreadLoadStatus.Failed)
            {
                throw new InvalidOperationException($"Failed to load resource ({path})");
            }

            await Task.Delay(50);
        }
    }
}