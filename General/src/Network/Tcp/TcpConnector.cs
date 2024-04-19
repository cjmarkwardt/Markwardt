namespace Markwardt;

[Factory<TcpConnector>]
public delegate ValueTask<IConnector<ReadOnlyMemory<byte>>> TcpConnectorFactory(IpAddress address);

public record TcpConnector(IpAddress Address) : Connector<ReadOnlyMemory<byte>>
{
    protected override async ValueTask<Failable<IConnection<ReadOnlyMemory<byte>>>> CreateConnection(CancellationToken cancellation = default)
    {
        TcpClient client = new();

        try
        {
            await client.ConnectAsync(Address.Host, Address.Port, cancellation);
        }
        catch (OperationCanceledException)
        {
            client.Dispose();
            return Failable.Cancel<IConnection<ReadOnlyMemory<byte>>>();
        }
        catch (SocketException exception)
        {
            client.Dispose();
            return exception;
        }
        catch (ArgumentOutOfRangeException exception)
        {
            client.Dispose();
            return exception;
        }

        return new TcpConnection(client);
    }
}