namespace Markwardt.Godot;

public class AssetLink<T>([Inject<AssetLoaderTag>] ILoader loader, string path) : LoadLink<T>(loader, path);