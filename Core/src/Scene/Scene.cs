namespace Markwardt;

public interface IScene : IMultiDisposable
{
    IEnumerable<object> Children { get; }

    void Add(object child);
    void Remove(object child);

    interface IModifier
    {
        void AddChild(object child);
        void RemoveChild(object child);
    }
}