namespace Markwardt;

public class GodotNodeFinder(Node root) : INodeFinder
{
    public INode Find(string path)
        => root.FindChild(path).Generalize();
}