namespace Markwardt;

public class TcpConnection : Connection<ReadOnlyMemory<byte>>
{
    public TcpConnection(TcpClient client, TimeSpan timeout, TimeSpan poll)
    {
        this.client = client;
        stream = client.GetStream();

        this.LoopInBackground(Read).DisposeWith(loops);
        this.LoopInBackground(TimeSpan.FromSeconds(0.5f), _ => { DetectTimeout(timeout); return ValueTask.CompletedTask; }).DisposeWith(loops);
        this.LoopInBackground(poll, _ => { Send(Array.Empty<byte>()); return ValueTask.CompletedTask; }).DisposeWith(loops);

        Connect();
    }

    public TcpConnection(TcpClient client)
        : this(client, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1)) { }

    private readonly TcpClient client;
    private readonly Stream stream;
    private readonly SequentialExecutor executor = new();
    private readonly ExtendedDisposable loops = new();

    private DateTime lastReceive = DateTime.Now;

    protected override void ExecuteSend(ReadOnlyMemory<byte> data, NetworkDelivery delivery)
        => this.Fork(async (_, _) => await executor.Execute(async () =>
        {
            await stream.WriteAsync(BitConverter.GetBytes((ushort)data.Length));
            await stream.WriteAsync(data);
        }));

    protected override void ExecuteDisconnect()
    {
        loops.Dispose();
        stream.Dispose();
        client.Dispose();
    }

    protected override void OnPrepareDisposal()
    {
        base.OnPrepareDisposal();

        client.DisposeWith(this);
        stream.DisposeWith(this);
        executor.DisposeWith(this);
    }

    private async ValueTask Read(CancellationToken cancellation)
    {
        Failable<Memory<byte>> tryReadLength = (await ReadData(2, cancellation)).WithLogging(this, "Failed to read data length");
        if (tryReadLength.IsFailure())
        {
            return;
        }

        lastReceive = DateTime.Now;

        ushort length = BitConverter.ToUInt16(tryReadLength.Result.Span);
        if (length > 0)
        {
            Failable<Memory<byte>> tryReadData = (await ReadData(length, cancellation)).WithLogging(this, "Failed to read data");
            if (tryReadLength.IsFailure())
            {
                return;
            }

            Receive(tryReadData.Result);
        }
    }

    private void DetectTimeout(TimeSpan timeout)
    {
        if (DateTime.Now > lastReceive + timeout)
        {
            Disconnect();
        }
    }

    private async ValueTask<Failable<Memory<byte>>> ReadData(int count, CancellationToken cancellation)
    {
        byte[] data = new byte[count];
        try
        {
            await stream.ReadExactlyAsync(data, cancellation);
        }
        catch (OperationCanceledException exception)
        {
            return exception;
        }
        catch (EndOfStreamException exception)
        {
            Disconnect();
            return exception;
        }

        return new Memory<byte>(data);
    }
}