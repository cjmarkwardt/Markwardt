namespace Markwardt;

public class TcpHost : AsyncHost<ReadOnlyMemory<byte>>
{
    public TcpHost(bool isListening, TcpListener listener)
        : base(isListening)
    {
        this.listener = listener;

        Initialize();
    }

    private readonly TcpListener listener;

    protected override async ValueTask<Failable<IConnection<ReadOnlyMemory<byte>>>> Listen(CancellationToken cancellation)
    {
        TcpClient client;

        try
        {
            client = await listener.AcceptTcpClientAsync(cancellation);
        }
        catch (OperationCanceledException)
        {
            return Failable.Cancel<IConnection<ReadOnlyMemory<byte>>>();
        }

        return new TcpConnection(client);
    }
}