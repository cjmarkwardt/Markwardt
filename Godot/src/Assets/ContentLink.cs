namespace Markwardt.Godot;

public class ContentLink<T>([Inject<ContentLoaderTag>] ILoader loader, string path) : LoadLink<T>(loader, path);