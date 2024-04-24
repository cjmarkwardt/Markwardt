namespace Markwardt;

public interface INode : IScene
{
    IScene? Parent { get; }
}