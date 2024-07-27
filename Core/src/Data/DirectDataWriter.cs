namespace Markwardt;

public class DirectDataTransformer
{
    private readonly IDynamicBuffer valueBuffer = new DynamicBuffer(8);

    public IDynamicBuffer Buffer { get; } = new DynamicBuffer(64);

    public Stream? Stream { get; set; }

    public void WriteKind(DataKind kind)
        => Stream.NotNull().WriteByte((byte)kind);

    public DataKind ReadKind()
        => (DataKind)Stream.NotNull().ReadByte();

    public void WriteSingle(float value)
    {
        BitConverter.TryWriteBytes(valueBuffer.Data, value);
        Stream.NotNull().Write(valueBuffer.Data[..4]);
    }

    public float ReadSingle()
    {
        Stream.NotNull().Read(valueBuffer.Data[..4]);
        return BitConverter.ToSingle(valueBuffer.Data);
    }

    public void WriteDouble(double value)
    {
        BitConverter.TryWriteBytes(valueBuffer.Data, value);
        Stream.NotNull().Write(valueBuffer.Data[..8]);
    }

    public double ReadDouble()
    {
        Stream.NotNull().Read(valueBuffer.Data[..8]);
        return BitConverter.ToDouble(valueBuffer.Data);
    }

    public void WriteInteger(BigInteger value)
    {
        valueBuffer.Length = value.GetByteCount();
        value.TryWriteBytes(valueBuffer.Data, out _);
        Stream.NotNull().WriteByte((byte)valueBuffer.Length);
        Stream.NotNull().Write(valueBuffer.Data);
    }

    public BigInteger ReadInteger()
    {
        valueBuffer.Length = Stream.NotNull().ReadByte();
        Stream.NotNull().Read(valueBuffer.Data);
        return new BigInteger(valueBuffer.Data);
    }

    public void WriteBlock(ReadOnlySpan<byte> source)
    {
        WriteInteger(source.Length);
        Stream.NotNull().Write(source);
    }

    public byte[] ReadBlock()
    {
        byte[] block = new byte[(int)ReadInteger()];
        Stream.NotNull().Read(block);
        return block;
    }

    public void WriteBufferBlock()
        => WriteBlock(Buffer.Data);

    public void ReadBufferBlock()
    {
        Buffer.Length = (int)ReadInteger();
        Stream.NotNull().Read(Buffer.Data);
    }

    public void WriteString(string value)
    {        
        Buffer.Length = Encoding.UTF8.GetByteCount(value);
        Encoding.UTF8.GetBytes(value, Buffer.Data);
        WriteBufferBlock();
    }

    public string ReadString()
    {
        ReadBufferBlock();
        return Encoding.UTF8.GetString(Buffer.Data);
    }
}