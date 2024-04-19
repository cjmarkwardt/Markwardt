namespace Markwardt;

public class SteamConnectionHandle(HSteamNetConnection value) : Handle<HSteamNetConnection>(value, x => SteamNetworkingSockets.CloseConnection(x, 0, string.Empty, true));