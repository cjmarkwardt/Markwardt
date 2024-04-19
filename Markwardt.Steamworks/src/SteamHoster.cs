namespace Markwardt;

[Factory<SteamHoster>]
public delegate ValueTask<IHoster<ReadOnlyMemory<byte>>> SteamHosterFactory(int virtualPort = 0);

public record SteamHoster(int VirtualPort = 0) : Hoster<ReadOnlyMemory<byte>>
{
    public required ISteamInitializer Initializer { get; init; }

    protected override ValueTask<Failable<IHost<ReadOnlyMemory<byte>>>> CreateHost(bool isListening = true, CancellationToken cancellation = default)
    {
        Failable tryInitialize = Initializer.Initialize();
        if (tryInitialize.Exception is not null)
        {
            return tryInitialize.Exception.AsFailable<IHost<ReadOnlyMemory<byte>>>();
        }

        return new SteamHost(isListening, () => SteamNetworkingSockets.CreateListenSocketP2P(VirtualPort, 0, [])).AsFailable<IHost<ReadOnlyMemory<byte>>>();
    }
}