namespace Markwardt;

public abstract class AsyncHost<T>(bool isListening) : Host<T>(isListening)
    where T : notnull
{
    protected abstract ValueTask<Failable<IConnection<T>>> Listen(CancellationToken cancellation);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        this.LoopInBackground(async (_, cancellation) =>
        {
            Failable<IConnection<T>> tryListen = await Listen(cancellation);
            if (tryListen.IsSuccess())
            {
                Receive(tryListen.Result);
            }
        });
    }
}