namespace Markwardt;

public class LiteNetHost : Host<ReadOnlyMemory<byte>>
{
    public LiteNetHost(bool isListening)
        : base(isListening)
    {
        controller.Connected.Subscribe(peer => Receive(new LiteNetConnection(controller, peer))).DisposeWith(this);

        Initialize();
    }

    private readonly LiteNetController controller = new();

    public bool Start(int port)
        => controller.Network.Start(port);

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        controller.DisposeWith(this);
    }
}