namespace Markwardt;

public interface IDataSpaceWriter
{
    bool IsInitialized { get; }
    int HeaderSize { get; }
    int BlockSize { get; }

    ValueTask ReadHeader(Memory<byte> destination);
    ValueTask WriteHeader(ReadOnlyMemory<byte> source);
    ValueTask ReadBlock(int index, Memory<byte> destination);
    ValueTask WriteBlock(int index, ReadOnlyMemory<byte> source);
}