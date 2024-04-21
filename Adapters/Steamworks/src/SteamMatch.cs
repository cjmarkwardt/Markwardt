namespace Markwardt;

public class SteamMatch : Match
{
    public SteamMatch(CSteamID id)
    {
        this.id = id;

        RefreshData();
    }

    private readonly CSteamID id;

    public override string Id => id.m_SteamID.ToString();

    public override async void Refresh()
    {
        if (SteamMatchmaking.RequestLobbyData(id))
        {
            Failable<LobbyDataUpdate_t> tryCallback = await Steam.Callback<LobbyDataUpdate_t>(x => x.m_ulSteamIDLobby == id.m_SteamID);
            if (!tryCallback.IsFailure())
            {
                RefreshData();
            }
        }
    }

    private void RefreshData()
    {
        Dictionary<string, string> properties = [];
        for (int i = 0; i < SteamMatchmaking.GetLobbyDataCount(id); i++)
        {
            if (SteamMatchmaking.GetLobbyDataByIndex(id, i, out string key, 250, out string value, 500))
            {
                properties[key] = value;
            }
        }

        SetProperties(properties);
    }
}