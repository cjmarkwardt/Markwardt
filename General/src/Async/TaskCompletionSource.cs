namespace Markwardt;

public class TaskCompletionSource
{
    public TaskCompletionSource(TaskCreationOptions creationOptions)
    {
        source = new(creationOptions);
    }

    public TaskCompletionSource()
    {
        source = new();
    }

    private readonly TaskCompletionSource<object?> source;

    public Task Task => source.Task;

    public void SetException(IEnumerable<Exception> exceptions)
        => source.SetException(exceptions);

    public void SetException(Exception exception)
        => source.SetException(exception);

    public void SetCanceled()
        => source.SetCanceled();

    public void SetResult()
        => source.SetResult(null);

    public bool TrySetCanceled()
        => source.TrySetCanceled();

    public bool TrySetCanceled(CancellationToken cancellationToken)
        => source.TrySetCanceled(cancellationToken);

    public bool TrySetException(IEnumerable<Exception> exceptions)
        => source.TrySetException(exceptions);

    public bool TrySetException(Exception exception)
        => source.TrySetException(exception);

    public bool TrySetResult()
        => source.TrySetResult(null);
}