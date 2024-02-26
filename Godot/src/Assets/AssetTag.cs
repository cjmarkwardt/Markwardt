namespace Markwardt.Godot;

public abstract class AssetTag<T> : LoadTag<AssetLoaderTag, T>
{
    protected override ILoadLink<T> CreateLink(ILoader loader, string path)
        => new AssetLink<T>(loader, path);
}