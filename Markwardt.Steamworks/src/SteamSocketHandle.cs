namespace Markwardt;

public class SteamSocketHandle(HSteamListenSocket value) : Handle<HSteamListenSocket>(value, x => SteamNetworkingSockets.CloseListenSocket(x));