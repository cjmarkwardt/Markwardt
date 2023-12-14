namespace Markwardt;

public interface IComponentParentManager : IDisposable
{
    IComponent? Parent { get; set; }
}

public sealed class ComponentParentManager(IComponent component) : IComponentParentManager
{
    private bool isDisposed;
    private IDisposable? logRouting;

    private IComponent? parent;
    public IComponent? Parent
    {
        get => parent;
        set
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(component.GetType().FullName);
            }

            Detach();

            parent = value;

            if (parent != null)
            {
                Attach(parent);
            }
        }
    }

    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            Detach();
        }
    }
    
    private void Attach(IComponent parent)
    {
        parent.UntrackDisposable(component);
        logRouting?.Dispose();
    }

    private void Detach()
    {
        if (parent != null)
        {
            parent.TrackDisposable(component);
            logRouting = component.RouteLogsTo(parent);
        }
    }
}
