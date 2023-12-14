namespace Markwardt;

public interface IDisposalTracker
{
    void TrackDisposable(object? target);
    void UntrackDisposable(object? target);
}

public static class DisposalTrackerExtensions
{
    public static void TrackDisposable(this IDisposalTracker tracker, params object?[] targets)
    {
        foreach (object? target in targets)
        {
            tracker.TrackDisposable(target);
        }
    }
    
    public static void UntrackDisposable(this IDisposalTracker tracker, params object?[] targets)
    {
        foreach (object? target in targets)
        {
            tracker.UntrackDisposable(target);
        }
    }

    public static T DisposeWith<T>(this T disposable, IDisposalTracker tracker)
    {
        tracker.TrackDisposable(disposable);
        return disposable;
    }
}

public sealed class DisposalTracker : IDisposalTracker, IMultiDisposable
{
    private readonly HashSet<object> disposables = [];

    public void TrackDisposable(object? target)
    {
        if (target != null)
        {
            disposables.Add(target);
        }
    }

    public void UntrackDisposable(object? target)
    {
        if (target != null)
        {
            disposables.Remove(target);
        }
    }

    public void Dispose()
    {
        foreach (IDisposable disposable in disposables.OfType<IDisposable>())
        {
            disposable.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
        => await Task.WhenAll(disposables.OfType<IAsyncDisposable>().Select(x => x.DisposeAsync().AsTask()).ToList());
}