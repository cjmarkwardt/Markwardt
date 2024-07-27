namespace Markwardt;

public interface IDynamicBuffer
{
    Span<byte> Data { get; }

    int Capacity { get; set; }
    int Length { get; set; }

    void Reset();
}

public static class DynamicBufferExtensions
{
    public static bool IsOverflow(this IDynamicBuffer buffer, int length)
        => length > buffer.Capacity;

    public static void Enlarge(this IDynamicBuffer buffer, int length)
    {
        Span<byte> oldData = buffer.Data;
        bool isOverflow = buffer.IsOverflow(length);
        buffer.Length = length;

        if (isOverflow)
        {
            oldData.CopyTo(buffer.Data);
        }
    }
    
    public static void Fill(this IDynamicBuffer buffer, ReadOnlySpan<byte> source)
    {
        buffer.Length = source.Length;
        source.CopyTo(buffer.Data);
    }
    
    public static void Append(this IDynamicBuffer buffer, ReadOnlySpan<byte> source)
    {
        int previousLength = buffer.Data.Length;
        int length = previousLength + source.Length;
        buffer.Enlarge(length);
        source.CopyTo(buffer.Data[previousLength..]);
    }

    public static void Clear(this IDynamicBuffer buffer)
        => buffer.Length = 0;
}

public class DynamicBuffer : IDynamicBuffer
{
    public DynamicBuffer(int defaultLength)
    {
        this.defaultLength = defaultLength;
        buffer = new byte[defaultLength];
    }

    public DynamicBuffer()
    {
        buffer = [];
    }

    private readonly int defaultLength;

    private byte[] buffer;
    private int length;

    public Span<byte> Data => buffer.AsSpan(0, length);

    public int Capacity
    {
        get => buffer.Length;
        set
        {
            if (value != Capacity)
            {
                buffer = new byte[value];
                length = 0;
            }
        }
    }

    public int Length
    {
        get => length;
        set
        {
            if (value > Capacity)
            {
                Capacity = value;
            }

            length = value;
        }
    }

    public void Reset()
        => Length = defaultLength;
}