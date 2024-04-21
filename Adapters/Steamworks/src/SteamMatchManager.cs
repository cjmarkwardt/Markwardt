namespace Markwardt;

public class SteamMatchManager(CSteamID id) : MatchManager
{
    public override string Id => id.m_SteamID.ToString();

    protected override void OnPropertySet(string key, string? value)
    {
        if (value is null)
        {
            SteamMatchmaking.DeleteLobbyData(id, key);
        }
        else
        {
            SteamMatchmaking.SetLobbyData(id, key, value);
        }
    }

    protected override void OnSharedDisposal()
    {
        base.OnSharedDisposal();

        SteamMatchmaking.LeaveLobby(id);
    }
}