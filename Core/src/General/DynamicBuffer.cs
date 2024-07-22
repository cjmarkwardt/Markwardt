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

    public void Prepare(int length)
    {
        if (buffer.Length < length)
        {
            Memory<byte> previousBuffer = buffer;
            buffer = new byte[length];

            Data.CopyTo(buffer);
            Data = buffer.AsMemory()[..Data.Length];
        }
    }

    public void FillFrom(ReadOnlySpan<byte> source)
    {
        Prepare(source.Length);
        Data = buffer.AsMemory()[..source.Length];
        source.CopyTo(Data.Span);
    }

    public void AppendFrom(ReadOnlySpan<byte> source)
    {
        int previousLength = Data.Length;
        int length = previousLength + source.Length;
        Prepare(length);
        Data = buffer.AsMemory()[..length];
        source.CopyTo(Data.Span.Slice(previousLength));
    }

    public void Clear()
        => Data = Memory<byte>.Empty;
}