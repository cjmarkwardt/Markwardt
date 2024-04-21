namespace Markwardt;

public class SteamConnection : Connection<ReadOnlyMemory<byte>>
{
    public SteamConnection(Func<HSteamNetConnection> createHandle)
    {
        Callback<SteamNetConnectionStatusChangedCallback_t>.Create(OnConnectionStatusChanged).DisposeWith(this);

        handle = new SteamConnectionHandle(createHandle()).DisposeWith(this);

        if (!SteamNetworkingSockets.GetConnectionInfo(handle.Value, out SteamNetConnectionInfo_t info))
        {
            Disconnect("Connection handle is invalid");
        }
        else
        {
            TransitionState(info);
        }

        this.LoopInBackground(TimeSpan.FromMilliseconds(25), _ => { Receive(); return ValueTask.CompletedTask; });
    }

    private readonly SteamConnectionHandle? handle;

    protected override void ExecuteSend(ReadOnlyMemory<byte> message, NetworkDelivery delivery)
    {
        if (handle is not null && handle.IsValid)
        {
            using ArrayHandle data = new(message);
            EResult result = SteamNetworkingSockets.SendMessageToConnection(handle.Value, data.Value, (uint)data.Length, delivery is NetworkDelivery.Reliable ? 8 : 0, out _);
            if (result is not EResult.k_EResultOK)
            {
                Disconnect($"Failed to send ({result})");
            }
        }
    }

    protected override void ExecuteDisconnect()
    {
        if (handle is not null && handle.IsValid)
        {
            handle.Dispose();
        }
    }

    private void OnConnectionStatusChanged(SteamNetConnectionStatusChangedCallback_t callback)
    {
        if (handle is not null && handle.IsValid && callback.m_hConn == handle.Value)
        {
            TransitionState(callback.m_info);
        }
    }

    private void TransitionState(SteamNetConnectionInfo_t status)
    {
        if (status.m_eState is ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_Connected)
        {
            Connect();
        }
        else if (status.m_eState is ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ClosedByPeer || status.m_eState is ESteamNetworkingConnectionState.k_ESteamNetworkingConnectionState_ProblemDetectedLocally)
        {
            Disconnect(status.m_szEndDebug);
        }
    }

    private void Receive()
    {
        if (handle is not null && handle.IsValid)
        {
            using ObservableList<SteamMessageHandle> messages = new() { ItemDisposal = ItemDisposal.Full };

            while (true)
            {
                nint[] messagePointers = new nint[100];
                int messageCount = SteamNetworkingSockets.ReceiveMessagesOnConnection(handle.Value, messagePointers, messagePointers.Length);
                if (messageCount < 0)
                {
                    Disconnect($"Failed to receive");
                    return;
                }
                else if (messageCount == 0)
                {
                    break;
                }
                else
                {
                    messages.Add(messagePointers.Take(messageCount).Select(x => new SteamMessageHandle(x)));
                }
            }

            foreach (SteamMessageHandle message in messages)
            {
                Receive(message.Data.Value);
            }
        }

        return;
    }
}