namespace Markwardt;

public interface IManagedDisposable : IDisposalViewer, IDisposalTracker;

public class ManagedDisposable : ReactiveObject, IManagedDisposable
{
    protected DisposalTracker Tracker { get; } = new();

    public bool IsDisposed { get; protected set; }

    public void TrackDisposable(object? target)
        => Tracker.TrackDisposable(target);

    public void UntrackDisposable(object? target)
        => Tracker.UntrackDisposable(target);

    public void Verify()
    {
        if (IsDisposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            PrepareDisposal();
            OnDisposal();
            Tracker.Dispose();
        }
    }

    protected virtual void PrepareDisposal() { }
    protected virtual void OnDisposal() { }
}