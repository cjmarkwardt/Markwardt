namespace Markwardt;

public class SteamHost : Host<ReadOnlyMemory<byte>>
{
    public SteamHost(bool isListening, Func<HSteamListenSocket> createHandle)
        : base(isListening)
    {
        Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionStatusChanged).DisposeWith(this);

        handle = new SteamSocketHandle(createHandle()).DisposeWith(this);

        Initialize();
    }

    private readonly SteamSocketHandle handle;

    private void OnConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t callback)
    {
        if (callback.m_info.m_hListenSocket != handle.Value)
        {
            return;
        }

        if (callback.m_eOldState is ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_None && callback.m_info.m_eState is ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connecting)
        {
            if (SteamNetworkingSockets.AcceptConnection(callback.m_hConn) is EResult.k_EResultOK)
            {
                Receive(new SteamConnection(() => callback.m_hConn));
            }
            else
            {
                SteamNetworkingSockets.CloseConnection(callback.m_hConn, 0, string.Empty, false);
            }
        }
    }
}