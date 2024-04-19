namespace Markwardt;

[Factory<SteamLocalConnector>]
public delegate ValueTask<IConnector<ReadOnlyMemory<byte>>> SteamLocalConnectorFactory(int virtualPort = 0);

public record SteamLocalConnector(int VirtualPort = 0) : IConnector<ReadOnlyMemory<byte>>
{
    public required ISteamInitializer Initializer { get; init; }

    public async ValueTask<Failable<IConnection<ReadOnlyMemory<byte>>>> Connect(CancellationToken cancellation = default)
    {
        Failable tryInitialize = Initializer.Initialize();
        if (tryInitialize.Exception is not null)
        {
            return tryInitialize.Exception;
        }

        return await new SteamConnector(new SteamAddress(SteamUser.GetSteamID().m_SteamID, VirtualPort)) { Initializer = Initializer }.Connect(cancellation);
    }
}