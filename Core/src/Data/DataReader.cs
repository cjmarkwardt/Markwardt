namespace Markwardt;

public interface IDataReader
{
    object? Read(IDynamicBuffer? buffer = null);

    bool? ReadBoolean()
        => (bool?)Read();

    BigInteger? ReadInteger()
        => (BigInteger?)Read();

    byte? ReadByte()
        => (byte?)Read();

    sbyte? ReadSignedByte()
        => (sbyte?)Read();

    short? ReadShort()
        => (short?)Read();

    ushort? ReadUnsignedShort()
        => (ushort?)Read();

    int? ReadInt()
        => (short?)Read();

    uint? ReadUnsignedInt()
        => (ushort?)Read();

    long? ReadLong()
        => (long?)Read();

    ulong? ReadUnsignedLong()
        => (ulong?)Read();

    float? ReadSingle()
        => (float?)Read();

    double? ReadDouble()
        => (double?)Read();

    Memory<byte>? ReadBlock(IDynamicBuffer? buffer = null)
        => (Memory<byte>?)Read(buffer);
}