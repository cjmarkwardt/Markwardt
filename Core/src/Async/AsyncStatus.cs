namespace Markwardt;

public class AsyncStatus
{
    public AsyncStatus()
    {
        completion.SetResult();
    }

    private TaskCompletionSource completion = new();
    private int processes = 0;

    public Task Task => completion.Task;

    public IDisposable Start()
    {
        if (processes == 0)
        {
            completion = new();
        }

        processes++;

        return Disposable.Create(() =>
        {
            processes--;

            if (processes == 0)
            {
                completion.SetResult();
            }
        });
    }
}