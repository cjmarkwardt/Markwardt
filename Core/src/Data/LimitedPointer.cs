namespace Markwardt;

public class LimitedPointer(IDataPointer data, int min, int max) : IDataPointer
{
    public int Position { get; set; }

    public IDataPointer Copy()
        => new LimitedPointer(data, min, max) { Position = Position};

    public async ValueTask Read(int length, Memory<byte> destination, bool move = false)
    {
        Validate(length);
        await data.Read(length, destination, move);
    }

    public async ValueTask Write(ReadOnlyMemory<byte> source, bool move = false)
    {
        Validate(source.Length);
        await data.Write(source, move);
    }

    private void Validate(int length)
    {
        if (Position < min || Position + length > max)
        {
            throw new InvalidOperationException($"Failed to read {Position}-{Position + length} because data pointer is limited to {min}-{max}");
        }
    }
}