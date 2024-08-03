namespace Markwardt;

public interface IDataReader
{
    (object? Value, string? Type) Read(IDynamicBuffer? buffer = null);

    (bool? Value, string? Type) ReadBoolean()
    {
        (object? value, string? type) = Read();
        return ((bool?)value, type);
    }

    (BigInteger? Value, string? Type) ReadInteger()
    {
        (object? value, string? type) = Read();
        return ((BigInteger?)value, type);
    }

    (byte? Value, string? Type) ReadByte()
    {
        (object? value, string? type) = Read();
        return ((byte?)value, type);
    }

    (sbyte? Value, string? Type) ReadSignedByte()
    {
        (object? value, string? type) = Read();
        return ((sbyte?)value, type);
    }

    (short? Value, string? Type) ReadShort()
    {
        (object? value, string? type) = Read();
        return ((short?)value, type);
    }

    (ushort? Value, string? Type) ReadUnsignedShort()
    {
        (object? value, string? type) = Read();
        return ((ushort?)value, type);
    }

    (int? Value, string? Type) ReadInt()
    {
        (object? value, string? type) = Read();
        return ((int?)value, type);
    }

    (uint? Value, string? Type) ReadUnsignedInt()
    {
        (object? value, string? type) = Read();
        return ((uint?)value, type);
    }

    (long? Value, string? Type) ReadLong()
    {
        (object? value, string? type) = Read();
        return ((long?)value, type);
    }

    (ulong? Value, string? Type) ReadUnsignedLong()
    {
        (object? value, string? type) = Read();
        return ((ulong?)value, type);
    }

    (float? Value, string? Type) ReadSingle()
    {
        (object? value, string? type) = Read();
        return ((float?)value, type);
    }

    (double? Value, string? Type) ReadDouble()
    {
        (object? value, string? type) = Read();
        return ((double?)value, type);
    }

    (Memory<byte>? Value, string? Type) ReadBlock(IDynamicBuffer? buffer = null)
    {
        (object? value, string? type) = Read();
        return ((Memory<byte>?)value, type);
    }
}