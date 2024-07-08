namespace Markwardt;

public interface IDirectDataPointer
{
    int Position { get; }

    IDirectDataPointer Move(int offset);
    ValueTask Read(int start, int length, Memory<byte> destination);
    ValueTask Write(int start, ReadOnlyMemory<byte> source);
}