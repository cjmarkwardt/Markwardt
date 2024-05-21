namespace Markwardt;

public interface INode : IScene
{
    object? Parent { get; set; }

    new interface IModifier : IScene.IModifier
    {
        void SetParent(object? parent);
    }
}

public static class NodeExtensions
{
    public static object? GetRoot(this INode node)
    {
        object? current = node.Parent;
        while (current is INode currentNode)
        {
            current = currentNode.Parent;
        }

        return current;
    }

    public static void SetRootParent(this INode node)
        => node.Parent = node.GetRoot();
}