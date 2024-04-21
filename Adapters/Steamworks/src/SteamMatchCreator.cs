namespace Markwardt;

public class SteamMatchCreator : IMatchCreator
{
    public required ISteamInitializer Initializer { get; init; }

    public async ValueTask<Failable<IMatchManager>> CreateMatch(CancellationToken cancellation = default)
    {
        Failable tryInitialize = Initializer.Initialize();
        if (tryInitialize.Exception is not null)
        {
            return tryInitialize.Exception.AsFailable<IMatchManager>();
        }

        Failable<LobbyCreated_t> tryCreateLobby = await SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 250).Consume<LobbyCreated_t>(cancellation);
        if (tryCreateLobby.Exception is not null)
        {
            return tryCreateLobby.Exception;
        }

        LobbyCreated_t result = tryCreateLobby.Result;
        if (result.m_eResult is not EResult.k_EResultOK)
        {
            return Failable.Fail<IMatchManager>(tryCreateLobby.Result.m_eResult.ToString());
        }
        else if (result.m_ulSteamIDLobby == 0)
        {
            return Failable.Fail<IMatchManager>("Failed to create Steam lobby");
        }
        else
        {
            return new SteamMatchManager(new(result.m_ulSteamIDLobby));
        }
    }
}