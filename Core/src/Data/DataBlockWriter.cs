namespace Markwardt;

public class DataBlockWriter(IDataSpaceWriter writer, bool headerOnly)
{
    public static int HeaderSize => sizeof(bool) + sizeof(int);

    private readonly IDataSpaceWriter writer = writer;
    private readonly Memory<byte> data = new byte[headerOnly ? HeaderSize : writer.BlockSize];

    private bool isFinal;
    private int parameter;
    
    public int MaxContentSize => data.Length - HeaderSize;
    public Memory<byte> Content => isFinal ? data.Slice(HeaderSize, parameter) : data[HeaderSize..];
    public int NextBlock => isFinal ? -1 : parameter;

    public void SetFinal(int size)
    {
        isFinal = true;
        parameter = size;
    }

    public void SetContinuation(int nextBlock)
    {
        isFinal = false;
        parameter = nextBlock;
    }

    public async ValueTask Read(int index)
    {
        await writer.ReadBlock(index, data);

        isFinal = data.Span.AsReadOnly().ReadBool(out _);
        parameter = data[1..].Span.AsReadOnly().ReadInt(out _);
    }

    public async ValueTask Write(int index)
    {
        data.Span.WriteBool(isFinal, out _);
        data[1..].Span.WriteInt(parameter, out _);
        
        await writer.WriteBlock(index, data);
    }
}