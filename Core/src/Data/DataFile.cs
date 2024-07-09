namespace Markwardt;

/*public interface IDataSpace
{
    ValueTask<IDataFile> Create(int size = 0);
    IDataFile Get(int id);
}

public class DataSpace : IDataSpace
{
    public ValueTask<IDataFile> Create(int size = 0)
    {
        throw new NotImplementedException();
    }

    public IDataFile Get(int id)
    {
        throw new NotImplementedException();
    }
}

public interface IDataPoint
{
    ValueTask Write(ReadOnlyMemory<byte> source, int offset = 0, bool move = true);
    ValueTask Read(Memory<byte> destination, int length, int offset = 0, bool move = true);
}

public interface IDataFile
{
    int Id { get; }
    int Position { get; set; }

    ValueTask Write(ReadOnlyMemory<byte> source, int offset = 0, bool move = true);
    ValueTask Read(Memory<byte> destination, int length, int offset = 0, bool move = true);
    ValueTask Resize(int length);
    ValueTask Delete();
}*/