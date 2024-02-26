namespace Markwardt.Godot;

public abstract class ContentTag<T> : LoadTag<ContentLoaderTag, T>
{
    protected override ILoadLink<T> CreateLink(ILoader loader, string path)
        => new ContentLink<T>(loader, path);
}