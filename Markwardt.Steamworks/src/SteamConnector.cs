namespace Markwardt;

[Factory<SteamConnector>]
public delegate ValueTask<IConnector<ReadOnlyMemory<byte>>> SteamConnectorFactory(SteamAddress address);

public record SteamConnector(SteamAddress Address) : IConnector<ReadOnlyMemory<byte>>
{
    public required ISteamInitializer Initializer { get; init; }

    public ValueTask<Failable<IConnection<ReadOnlyMemory<byte>>>> Connect(CancellationToken cancellation = default)
    {
        Failable tryInitialize = Initializer.Initialize();
        if (tryInitialize.Exception is not null)
        {
            return tryInitialize.Exception.AsFailable<IConnection<ReadOnlyMemory<byte>>>();
        }

        HSteamNetConnection CreateHandle()
        {
            SteamNetworkingIdentity target = new();
            target.SetSteamID64(Address.Id);
            return SteamNetworkingSockets.ConnectP2P(ref target, Address.VirtualPort, 0, []);
        }

        return new SteamConnection(CreateHandle).AsFailable<IConnection<ReadOnlyMemory<byte>>>();
    }
}