namespace Markwardt;

public class AsyncLazy<T>(Func<ValueTask<T>> create)
{
    private readonly TaskCompletionSource<T> completion = new();

    public AsyncLazyState State { get; private set; } = AsyncLazyState.Lazy;

    public async ValueTask<T> Get()
    {
        if (State is AsyncLazyState.Lazy)
        {
            State = AsyncLazyState.Creating;
            T value = await create();
            State = AsyncLazyState.Created;
            completion.SetResult(value);
            return value;
        }
        else
        {
            return await completion.Task;
        }
    }

    public bool TrySyncGet([MaybeNullWhen(false)] out T value)
    {
        if (State is AsyncLazyState.Created)
        {
            value = completion.Task.Result;
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }

    public T SyncGet()
        => TrySyncGet(out T? value) ? value : throw new InvalidOperationException();
}