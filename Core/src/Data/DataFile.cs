namespace Markwardt;

public interface IDataSpacee
{
    ValueTask<(int Id, IDataFile File)> Create();
    IDataFile Open(int id);
    ValueTask Delete(int id);
}

public interface IDataFile
{
    ValueTask Write(int start, ReadOnlyMemory<byte> source);
    ValueTask Read(int start, int length, Memory<byte> destination);
    ValueTask Resize(int length);
    ValueTask Delete();
}