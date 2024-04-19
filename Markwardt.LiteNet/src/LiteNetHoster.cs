namespace Markwardt;

[Factory<LiteNetHoster>]
public delegate ValueTask<IHoster<ReadOnlyMemory<byte>>> LiteNetHosterFactory(int port);

public record LiteNetHoster(int Port) : Hoster<ReadOnlyMemory<byte>>
{
    protected override async ValueTask<Failable<IHost<ReadOnlyMemory<byte>>>> CreateHost(bool isListening = true, CancellationToken cancellation = default)
    {
        LiteNetHost host = new(isListening);
        if (!host.Start(Port))
        {
            await host.DisposeAsync();
            return Failable.Fail<IHost<ReadOnlyMemory<byte>>>($"Failed to host on port {Port}");
        }

        return host;
    }
}