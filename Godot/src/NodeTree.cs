namespace Markwardt.Godot;

public interface INodeTree : IEnumerable<Node>
{
    Node Descend(string path);
}

public class NodeTree(Node @base) : INodeTree
{
    public Node Base => @base;

    public Node Descend(string path)
        => Base.GetNode(path);

    public IEnumerator<Node> GetEnumerator()
        => Base.GetChildren().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}