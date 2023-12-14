namespace Markwardt;

public interface IManagedAsyncDisposable : IManagedDisposable, IMultiDisposable;

public class ManagedAsyncDisposable : ManagedDisposable, IManagedAsyncDisposable
{
    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            PrepareDisposal();
            await OnAsyncDisposal();
            await Tracker.DisposeAsync();
        }
    }

    protected override void PrepareDisposal()
    {
        base.PrepareDisposal();
        OnSharedDisposal();
    }

    protected virtual void OnSharedDisposal() { }
    
    protected virtual ValueTask OnAsyncDisposal()
        => default;
}