namespace Markwardt;

public static class SceneTreeExtensions
{
    public static Node AddScene(this SceneTree tree, string path)
    {
        Node scene = ResourceLoader.Load<PackedScene>(path).Instantiate();
        tree.Root.AddChildDeferred(scene);
        return scene;
    }

    public static Node SetScene(this SceneTree tree, string path)
    {
        foreach (Node child in tree.Root.GetChildren())
        {
            tree.Root.RemoveChildDeferred(child);
        }

        return tree.AddScene(path);
    }
}