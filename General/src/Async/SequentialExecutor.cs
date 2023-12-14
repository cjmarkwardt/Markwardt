namespace Markwardt;

public interface ISequentialExecutor : IMultiDisposable
{
    ValueTask<T> Execute<T>(Func<ValueTask<T>> action);
}

public static class SequentialExecutorExtensions
{
    public static async ValueTask Execute(this ISequentialExecutor executor, Action action)
        => await executor.Execute(() => { action(); return new ValueTask<object?>(null); });

    public static async ValueTask<T> Execute<T>(this ISequentialExecutor executor, Func<T> action)
        => await executor.Execute(() => new ValueTask<T>(action()));

    public static async ValueTask Execute(this ISequentialExecutor executor, Func<ValueTask> action)
        => await executor.Execute<object?>(async () => { await action(); return null; });
}

public class SequentialExecutor : ISequentialExecutor
{
    private TaskCompletionSource? lastExecute;
    private bool isDisposed;

    public async ValueTask<T> Execute<T>(Func<ValueTask<T>> action)
    {
        if (isDisposed)
        {
            throw new ObjectDisposedException(GetType().FullName);
        }

        TaskCompletionSource execute = new();

        if (lastExecute != null)
        {
            TaskCompletionSource previousExecute = lastExecute;
            lastExecute = execute;
            await previousExecute.Task;
        }
        else
        {
            lastExecute = execute;
        }

        T result = await action();
        execute.SetResult();

        if (execute == lastExecute)
        {
            lastExecute = null;
        }

        return result;
    }

    public void Dispose() { }

    public async ValueTask DisposeAsync()
    {
        if (!isDisposed)
        {
            isDisposed = true;

            if (lastExecute != null)
            {
                await lastExecute.Task;
            }
        }
    }
}