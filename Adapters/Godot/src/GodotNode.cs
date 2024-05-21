namespace Markwardt;

public class GodotNode(Node node) : ExtendedDisposable, INode
{
    private readonly Node node = node;

    public IEnumerable<object> Children => node.GetChildren().Select(x => x.Generalize());

    public object? Parent { get => node.GetParent()?.Generalize(); set => node.Reparent(GetNode(value)); }

    public void Add(object child)
        => node.AddChildDeferred(GetNode(child));

    public void Remove(object child)
        => node.RemoveChildDeferred(GetNode(child));

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        node.DisposeWith(this);
    }

    private Node GetNode(object? node)
        => node as Node ?? (node as GodotNode)?.node ?? throw new InvalidOperationException("Child must be of type Godot.Node");
}