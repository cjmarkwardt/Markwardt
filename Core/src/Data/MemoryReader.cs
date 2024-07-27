namespace Markwardt;

public interface IMemoryReader
{
    void Read(int length, IDynamicBuffer destination);
    
}

public class MemoryReader(ReadOnlyMemory<byte> source) : IMemoryReader
{
    private int position;

    public void Read(int length, IDynamicBuffer destination)
    {
        destination.Append(source.Slice(position, length).Span);
        position += length;
    }
}