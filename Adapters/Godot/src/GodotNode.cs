namespace Markwardt;

public class GodotNode(Node node) : ExtendedDisposable, INode
{
    public IEnumerable<INode> Children => node.GetChildren().Select(x => x.Generalize());

    public IScene? Parent => node.GetParent()?.Generalize();

    public void Add(INode child)
    {
        if (child is Node childNode)
        {
            node.AddChildDeferred(childNode);
        }
        else
        {
            throw new InvalidOperationException("Child must be of type Godot.Node");
        }
    }

    public void Remove(INode child)
    {
        if (child is Node childNode)
        {
            node.RemoveChildDeferred(childNode);
        }
    }

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        node.DisposeWith(this);
    }
}