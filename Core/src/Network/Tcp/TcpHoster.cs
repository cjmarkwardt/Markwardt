namespace Markwardt;

[Factory<TcpHoster>]
public delegate ValueTask<IHoster<ReadOnlyMemory<byte>>> TcpHosterFactory(int port);

public record TcpHoster(int Port) : Hoster<ReadOnlyMemory<byte>>
{
    protected override ValueTask<Failable<IHost<ReadOnlyMemory<byte>>>> CreateHost(bool isListening = true, CancellationToken cancellation = default)
    {
        TcpListener listener = new(IPAddress.Any, Port);

        try
        {
            listener.Start();
        }
        catch (SocketException exception)
        {
            listener.Dispose();
            return Failable.Fail<IHost<ReadOnlyMemory<byte>>>(exception);
        }

        return new TcpHost(isListening, listener).AsFailable<IHost<ReadOnlyMemory<byte>>>();
    }
}