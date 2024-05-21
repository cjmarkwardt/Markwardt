namespace Markwardt;

public class GodotNodeFinder : INodeFinder
{
    public INode Find(string path)
        => GodotControl.GetRoot().FindChild(path).Generalize();
}