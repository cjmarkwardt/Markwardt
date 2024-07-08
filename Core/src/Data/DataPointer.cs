namespace Markwardt;

public interface IDataPointer
{
    int Position { get; }

    IDataPointer Move(int offset);
    ValueTask<IDataPointer> Read(Memory<byte> destination, int length, int offset = 0, bool move = false);
    ValueTask<IDataPointer> Write(ReadOnlyMemory<byte> source, int offset = 0, bool move = false);
}

public static class DataPointerExtensions
{
    public static async ValueTask<(IDataPointer Data, byte[] Value)> Read(this IDataPointer data, int length, int offset = 0, bool move = false)
    {
        byte[] value = new byte[length];
        return (await data.Read(value, length, offset, move), value);
    }

    public static async ValueTask<(IDataPointer Data, byte Value)> ReadByte(this IDataPointer data, int offset = 0, bool move = false)
    {
        (data, byte[] value) = await data.Read(1, offset, move);
        return (data, value[0]);
    }

    public static async ValueTask<IDataPointer> WriteByte(this IDataPointer data, byte value, int offset = 0, bool move = false)
        => await data.Write(new byte[value], offset, move);

    public static async ValueTask<(IDataPointer Data, bool Value)> ReadBoolean(this IDataPointer data, int offset = 0, bool move = false)
    {
        (data, byte value) = await data.ReadByte(offset, move);
        return (data, value != 0);
    }

    public static async ValueTask<IDataPointer> WriteBoolean(this IDataPointer pointer, bool value, int offset = 0, bool move = false)
        => await pointer.WriteByte(value ? (byte)1 : (byte)0, offset, move);

    public static async ValueTask<(IDataPointer Data, short Value)> ReadShort(this IDataPointer data, int offset = 0, bool move = false)
    {
        (data, byte[] value) = await data.Read(2, offset, move);
        return (data, BitConverter.ToInt16(value));
    }

    public static async ValueTask<IDataPointer> WriteShort(this IDataPointer pointer, short value, int offset = 0, bool move = false)
        => await pointer.Write(BitConverter.GetBytes(value), offset, move);

    public static async ValueTask<(IDataPointer Data, int Value)> ReadInteger(this IDataPointer data, int offset = 0, bool move = false)
    {
        (data, byte[] value) = await data.Read(4, offset, move);
        return (data, BitConverter.ToInt32(value));
    }

    public static async ValueTask<IDataPointer> WriteInteger(this IDataPointer pointer, int value, int offset = 0, bool move = false)
        => await pointer.Write(BitConverter.GetBytes(value), offset, move);
}