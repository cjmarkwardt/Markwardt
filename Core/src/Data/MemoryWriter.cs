namespace Markwardt;

public interface IMemoryWriter
{
    void Write(ReadOnlySpan<byte> source);
}

public class MemoryWriter(Memory<byte> destination) : IMemoryWriter
{
    private int position;

    public void Write(ReadOnlySpan<byte> source)
    {
        source.CopyTo(destination[position..].Span);
        position += source.Length;
    }
}