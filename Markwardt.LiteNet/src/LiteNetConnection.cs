namespace Markwardt;

public class LiteNetConnection : Connection<ReadOnlyMemory<byte>>
{
    public LiteNetConnection(LiteNetController controller, NetPeer peer)
    {
        this.peer = peer;

        controller.Connected.Where(x => x == peer).Subscribe(_ => Connect()).DisposeWith(this);
        controller.Received.Where(x => x.Peer == peer).Select(x => x.Data).Subscribe(Receive).DisposeWith(this);
        controller.Disconnected.Where(x => x == peer).Subscribe(_ => Disconnect("Remote disconnection")).DisposeWith(this);

        if (peer.ConnectionState is LiteNetLib.ConnectionState.Connected)
        {
            Connect();
        }
    }

    private readonly NetPeer peer;

    protected override void ExecuteSend(ReadOnlyMemory<byte> data, NetworkDelivery delivery)
        => peer.Send(data.Span, GetDeliveryMethod(delivery));

    protected override void ExecuteDisconnect()
        => peer.Disconnect();

    protected override void OnSharedDisposal()
    {
        base.OnSharedDisposal();

        peer.Disconnect();
    }

    private DeliveryMethod GetDeliveryMethod(NetworkDelivery delivery)
        => delivery switch
        {
            NetworkDelivery.Simple => DeliveryMethod.Unreliable,
            NetworkDelivery.Reliable => DeliveryMethod.ReliableUnordered,
            NetworkDelivery.Ordered => DeliveryMethod.Sequenced,
            NetworkDelivery.Full => DeliveryMethod.ReliableOrdered,
            _ => throw new NotSupportedException(delivery.ToString())
        };
}