namespace Markwardt;

public static class InvokeExtensions
{
    public static void InvokeAll(this IEnumerable<Action> actions)
    {
        foreach (Action action in actions)
        {
            action();
        }
    }
    
    public static Task InvokeAll(this IEnumerable<Task> tasks)
        => Task.WhenAll(tasks);
    
    public static Task InvokeAll(this IEnumerable<Func<ValueTask>> actions)
        => Task.WhenAll(actions.Select(action => action.Invoke().AsTask()));

    public static void TryDispose(this object? target)
    {
        if (target is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
    
    public static async ValueTask TryDisposeAsync(this object? target)
    {
        if (target is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            target.TryDispose();
        }
    }
}