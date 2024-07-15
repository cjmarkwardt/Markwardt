namespace Markwardt;

public interface IDynamicAppendBuffer
{
    void AppendFrom(ReadOnlySpan<byte> source);
}

public interface IDynamicBuffer
{
    Memory<byte> Data { get; }

    void Prepare(int length);
    void FillFrom(ReadOnlySpan<byte> source);
    void AppendFrom(ReadOnlySpan<byte> source);
    void Clear();
}

public class DynamicBuffer : IDynamicBuffer
{
    public DynamicBuffer(int size)
    {
        buffer = new byte[size];
    }

    public DynamicBuffer()
    {
        buffer = [];
    }

    private byte[] buffer;

    public Memory<byte> Data { get; private set; }

    public int Length
    {
        get => Data.Length;
        set
        {
            if (Data.Length != value)
            {
                Data = new byte[value];
            }
        }
    }

    public void Prepare(int length)
    {
        if (buffer.Length < length)
        {
            buffer = new byte[length];
        }
    }

    public void FillFrom(ReadOnlySpan<byte> source)
    {
        Prepare(source.Length);
        Data = buffer.AsMemory().Slice(0, source.Length);
        source.CopyTo(Data.Span);
    }

    public void AppendFrom(ReadOnlySpan<byte> source)
    {
        int previousLength = Data.Length;
        int length = previousLength + source.Length;
        Prepare(length);
        Data = buffer.AsMemory().Slice(0, length);
        source.CopyTo(Data.Span.Slice(previousLength));
    }

    public void Clear()
        => Data = Memory<byte>.Empty;
}