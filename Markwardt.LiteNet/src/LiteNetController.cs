namespace Markwardt;

public class LiteNetController : ExtendedDisposable
{
    public LiteNetController()
    {
        EventBasedNetListener listener = new();
        listener.ConnectionRequestEvent += request => request.Accept();
        listener.PeerConnectedEvent += connected.OnNext;
        listener.PeerDisconnectedEvent += (peer, _) => disconnected.OnNext(peer);
        listener.NetworkReceiveEvent += (peer, reader, _, _) => received.OnNext((peer, reader.RawData.AsMemory().Slice(reader.UserDataOffset, reader.UserDataSize)));

        Network = new(listener);

        this.LoopInBackground(TimeSpan.FromSeconds(0.05), _ => { Network.PollEvents(); return ValueTask.CompletedTask; });
    }

    public NetManager Network { get; }

    private readonly Subject<NetPeer> connected = new();
    public IObservable<NetPeer> Connected => connected;

    private readonly Subject<(NetPeer, ReadOnlyMemory<byte>)> received = new();
    public IObservable<(NetPeer Peer, ReadOnlyMemory<byte> Data)> Received => received;

    private readonly Subject<NetPeer> disconnected = new();
    public IObservable<NetPeer> Disconnected => disconnected;

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        connected.DisposeWith(this);
        received.DisposeWith(this);
        disconnected.DisposeWith(this);
    }

    protected override void OnSharedDisposal()
    {
        base.OnSharedDisposal();

        Network.DisconnectAll();
        Network.Stop();
    }
}