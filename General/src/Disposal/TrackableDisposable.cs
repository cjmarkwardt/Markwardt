namespace Markwardt;

public interface ITrackableDisposable : IMultiDisposable
{
    bool IsDisposed { get; }
    CancellationToken DisposalToken { get; }
    string DisposalName { get; }
}

public static class TrackableDisposableExtensions
{
    public static void Verify(this ITrackableDisposable viewer)
        => ObjectDisposedException.ThrowIf(viewer.IsDisposed, viewer.DisposalName);
}