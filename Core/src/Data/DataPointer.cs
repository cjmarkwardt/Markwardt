namespace Markwardt;

public interface IDataPointer
{
    int Position { get; set; }

    IDataPointer Copy();
    ValueTask Read(Memory<byte> destination, int length, int offset = 0, bool move = false);
    ValueTask Write(ReadOnlyMemory<byte> source, int offset = 0, bool move = false);
}

public static class DataPointerExtensions
{
    public static IDataPointer Move(this IDataPointer data, int offset)
    {
        data.Position += offset;
        return data;
    }

    public static async ValueTask<byte[]> Read(this IDataPointer data, int length, int offset = 0, bool move = false)
    {
        byte[] value = new byte[length];
        await data.Read(value, length, offset, move);
        return value;
    }

    public static async ValueTask Write(this IDataPointer data, byte[] source, int offset = 0, bool move = false)
        => await data.Write((ReadOnlyMemory<byte>)source, offset, move);

    public static async ValueTask Write(this IDataPointer data, IEnumerable<byte> source, int offset = 0, bool move = false)
        => await data.Write(source.ToArray(), offset, move);

    public static async ValueTask<byte> Read(this IDataPointer data, int offset = 0, bool move = false)
        => (await data.Read(1, offset, move))[0];

    public static async ValueTask Write(this IDataPointer data, byte source, int offset = 0, bool move = false)
        => await data.Write(new byte[source], offset, move);

    public static async ValueTask<IList<T>> Read<T>(this IDataPointer data, int count, int valueLength, Func<ReadOnlyMemory<byte>, T> readValue, int offset = 0, bool move = false)
        => (await data.Read(count * valueLength, offset, move)).AsMemory().Chunk(valueLength).Select(x => readValue(x)).ToList();

    public static async ValueTask Write<T>(this IDataPointer data, IEnumerable<T> values, int valueLength, Action<T, Memory<byte>> writeValue, int offset = 0, bool move = false)
    {
        Memory<byte> block = new byte[values.Count() * valueLength];
        int i = 0;
        foreach (T value in values)
        {
            writeValue(value, block.Slice(i, valueLength));
            i += valueLength;
        }

        await data.Write(block, offset, move);
    }

    public static async ValueTask<IList<bool>> ReadBooleans(this IDataPointer data, int count, int offset = 0, bool move = false)
        => await data.Read(count, 1, source => source.Span[0] != 0, offset, move);

    public static async ValueTask WriteBooleans(this IDataPointer data, IEnumerable<bool> values, int offset = 0, bool move = false)
        => await data.Write(values, 1, (x, destination) => destination.Span[0] = x.ToByte(), offset, move);

    public static async ValueTask<bool> ReadBoolean(this IDataPointer data, int offset = 0, bool move = false)
        => (await data.ReadBooleans(1, offset, move))[0];

    public static async ValueTask WriteBoolean(this IDataPointer data, bool value, int offset = 0, bool move = false)
        => await data.WriteBooleans([value], offset, move);

    public static async ValueTask<IList<short>> ReadShorts(this IDataPointer data, int count, int offset = 0, bool move = false)
        => await data.Read(count, 2, source => BitConverter.ToInt16(source.Span), offset, move);

    public static async ValueTask WriteShorts(this IDataPointer data, IEnumerable<short> values, int offset = 0, bool move = false)
        => await data.Write(values, 2, (x, destination) => BitConverter.TryWriteBytes(destination.Span, x), offset, move);

    public static async ValueTask<short> ReadShort(this IDataPointer data, int offset = 0, bool move = false)
        => (await data.ReadShorts(1, offset, move))[0];

    public static async ValueTask WriteShort(this IDataPointer data, short value, int offset = 0, bool move = false)
        => await data.WriteShorts([value], offset, move);

    public static async ValueTask<IList<int>> ReadIntegers(this IDataPointer data, int count, int offset = 0, bool move = false)
        => await data.Read(count, 4, source => BitConverter.ToInt32(source.Span), offset, move);

    public static async ValueTask WriteIntegers(this IDataPointer data, IEnumerable<int> values, int offset = 0, bool move = false)
        => await data.Write(values, 4, (x, destination) => BitConverter.TryWriteBytes(destination.Span, x), offset, move);

    public static async ValueTask<int> ReadInteger(this IDataPointer data, int offset = 0, bool move = false)
        => (await data.ReadIntegers(1, offset, move))[0];

    public static async ValueTask WriteInteger(this IDataPointer data, int value, int offset = 0, bool move = false)
        => await data.WriteIntegers([value], offset, move);
}