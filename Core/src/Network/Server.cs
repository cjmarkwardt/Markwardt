namespace Markwardt;

public interface IServer<TMessage> : IMultiDisposable
    where TMessage : notnull
{
    IObservableConsumptionQueue<IConnection<TMessage>> Receives { get; }

    IDisposable Host(IHost<TMessage> host, bool disposeHost = true);
}

public static class ServerExtensions
{
    public static async ValueTask<Failable<IDisposable>> Host<TMessage>(this IServer<TMessage> server, IHoster<TMessage> hoster, bool isListening = true, CancellationToken cancellation = default)
        where TMessage : notnull
    {
        Failable<IHost<TMessage>> tryHost = await hoster.Host(isListening, cancellation);
        if (tryHost.Exception is not null)
        {
            return tryHost.Exception;
        }

        return server.Host(tryHost.Result).AsFailable();
    }
}

public class Server<TMessage> : ExtendedDisposable, IServer<TMessage>
    where TMessage : notnull
{
    private readonly ObservableQueue<IConnection<TMessage>> receives = [];
    public IObservableConsumptionQueue<IConnection<TMessage>> Receives => receives;

    public IDisposable Host(IHost<TMessage> host, bool disposeHost = true)
    {
        if (disposeHost)
        {
            host.DisposeWith(this);
        }

        IDisposable subscription = host.Receives.Consume().Subscribe(x => receives.Enqueue(x)).DisposeWith(this);

        return Disposable.Create(() =>
        {
            this.DisposeInBackground(subscription);

            if (disposeHost)
            {
                this.DisposeInBackground(host);
            }
        });
    }

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        receives.DisposeWith(this);
    }
}