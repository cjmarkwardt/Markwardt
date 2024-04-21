namespace Markwardt;

[Factory<LiteNetConnector>]
public delegate ValueTask<IConnector<ReadOnlyMemory<byte>>> LiteNetConnectorFactory(IpAddress address);

public record LiteNetConnector(IpAddress Address) : Connector<ReadOnlyMemory<byte>>
{
    protected override async ValueTask<Failable<IConnection<ReadOnlyMemory<byte>>>> CreateConnection(CancellationToken cancellation = default)
    {
        LiteNetController controller = new();

        if (!controller.Network.Start())
        {
            await controller.DisposeAsync();
            return Failable.Fail<IConnection<ReadOnlyMemory<byte>>>("Failed to start networking");
        }

        LiteNetConnection connection = new(controller, controller.Network.Connect(Address.Host, Address.Port, string.Empty));
        controller.DisposeWith(connection);
        return connection;
    }
}