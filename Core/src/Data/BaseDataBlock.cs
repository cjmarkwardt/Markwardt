namespace Markwardt;

public class DataBlock(IDataSpaceWriter writer, bool headerOnly)
{
    public static int HeaderSize => sizeof(bool) + sizeof(int);

    private readonly IDataSpaceWriter writer = writer;
    private readonly Memory<byte> data = new byte[headerOnly ? HeaderSize : writer.BlockSize];

    public DataBlockType Type { get; set; }
    public int Parameter { get; set; }
    
    public int MaxContentSize => writer.BlockSize - HeaderSize;
    public Memory<byte> Content => data[HeaderSize..];

    public async ValueTask Read(int index)
    {
        await writer.ReadBlock(index, data);

        Type = (DataBlockType)data.Span.AsReadOnly().ReadByte(out _);
        Parameter = data[1..].Span.AsReadOnly().ReadInt(out _);
    }

    public async ValueTask Write(int index)
    {
        data.Span.WriteByte((byte)Type, out _);
        data[1..].Span.WriteInt(Parameter, out _);
        
        await writer.WriteBlock(index, data);
    }
}