namespace Markwardt;

public static class GodotControl
{
    public static SceneTree GetTree()
        => (SceneTree)Engine.GetMainLoop();

    public static void Quit()
        => GetTree().Quit();

    public static Window GetRoot()
        => GetTree().Root;

    public static T Load<T>(string path)
        where T : class
        => GD.Load<T>(path) ?? throw new InvalidOperationException($"Asset not found at {path}");

    public static T LoadNode<T>(string path)
        where T : Node
        => (T)Load<PackedScene>(path).Instantiate();
}