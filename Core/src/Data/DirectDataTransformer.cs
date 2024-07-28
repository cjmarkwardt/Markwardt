namespace Markwardt;

public class DirectDataTransformer
{
    private readonly IDynamicBuffer valueBuffer = new DynamicBuffer(8);
    private readonly IDynamicBuffer blockBuffer = new DynamicBuffer(64);

    public Stream? Stream { get; set; }

    public void WriteKind(DataKind kind)
        => Stream.NotNull().WriteByte((byte)kind);

    public DataKind ReadKind()
        => (DataKind)Stream.NotNull().ReadByte();

    public void WriteSingle(float value)
    {
        BitConverter.TryWriteBytes(valueBuffer.Data.Span, value);
        Stream.NotNull().Write(valueBuffer.Data[..4].Span);
    }

    public float ReadSingle()
    {
        Stream.NotNull().Read(valueBuffer.Data[..4].Span);
        return BitConverter.ToSingle(valueBuffer.Data.Span);
    }

    public void WriteDouble(double value)
    {
        BitConverter.TryWriteBytes(valueBuffer.Data.Span, value);
        Stream.NotNull().Write(valueBuffer.Data[..8].Span);
    }

    public double ReadDouble()
    {
        Stream.NotNull().Read(valueBuffer.Data[..8].Span);
        return BitConverter.ToDouble(valueBuffer.Data.Span);
    }

    public void WriteInteger(BigInteger value)
    {
        valueBuffer.Length = value.GetByteCount();
        value.TryWriteBytes(valueBuffer.Data.Span, out _);
        Stream.NotNull().WriteByte((byte)valueBuffer.Length);
        Stream.NotNull().Write(valueBuffer.Data.Span);
    }

    public BigInteger ReadInteger()
    {
        valueBuffer.Length = Stream.NotNull().ReadByte();
        Stream.NotNull().Read(valueBuffer.Data.Span);
        return new BigInteger(valueBuffer.Data.Span);
    }

    public void WriteBlock(ReadOnlySpan<byte> source)
    {
        WriteInteger(source.Length);
        Stream.NotNull().Write(source);
    }

    public void WriteBlock(Action<IDynamicBuffer> write)
    {
        write(blockBuffer);
        WriteBlock(blockBuffer.Data.Span);
    }

    public Memory<byte> ReadBlock(IDynamicBuffer? buffer = null)
    {
        buffer ??= blockBuffer;
        buffer.Length = (int)ReadInteger();
        Stream.NotNull().Read(buffer.Data.Span);
        return buffer.Data;
    }

    public void WriteString(string value)
        => WriteBlock(x =>
        {
            x.Length = Encoding.UTF8.GetByteCount(value);
            Encoding.UTF8.GetBytes(value, x.Data.Span);
        });

    public string ReadString()
        => Encoding.UTF8.GetString(ReadBlock().Span);
}