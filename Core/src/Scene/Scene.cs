namespace Markwardt;

public interface IScene : IMultiDisposable
{
    IEnumerable<INode> Children { get; }

    void Add(INode child);
    void Remove(INode child);
}