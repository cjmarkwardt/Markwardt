namespace Markwardt.Godot;

public abstract class LoadTag<TLoaderTag, T> : IServiceTag
{
    public IServiceDescription? Default => Service.FromSourceTag<TLoaderTag, ILoader>(ServiceKind.Singleton, loader => CreateLink(loader, Path));

    protected abstract string Path { get; }

    protected abstract ILoadLink<T> CreateLink(ILoader loader, string path);
}