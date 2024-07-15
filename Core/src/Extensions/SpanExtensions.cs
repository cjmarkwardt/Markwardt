namespace Markwardt;

public static class SpanExtensions
{
    public static ReadOnlySpan<byte> AsReadOnly(this Span<byte> span)
        => (ReadOnlySpan<byte>)span;

    public static ReadOnlyMemory<byte> AsReadOnly(this Memory<byte> span)
        => (ReadOnlyMemory<byte>)span;

    public static void WriteBytes(this Span<byte> span, ReadOnlySpan<byte> source, out int length)
    {
        length = source.Length;
        source.CopyTo(span);
    }

    public static void ReadBytes(this ReadOnlySpan<byte> span, Span<byte> destination, out int length)
    {
        length = destination.Length;
        span.CopyTo(destination);
    }

    public static void WriteByte(this Span<byte> span, byte value, out int length)
    {
        length = 1;
        span[0] = value;
    }

    public static byte ReadByte(this ReadOnlySpan<byte> span, out int length)
    {
        length = 1;
        return span[0];
    }

    public static void WriteBool(this Span<byte> span, bool value, out int length)
        => span.WriteByte(value ? (byte)1 : (byte)0, out length);

    public static bool ReadBool(this ReadOnlySpan<byte> span, out int length)
        => span.ReadByte(out length) == 1;

    public static void WriteShort(this Span<byte> span, short value, out int length)
    {
        length = 2;
        BitConverter.TryWriteBytes(span, value);
    }

    public static short ReadShort(this ReadOnlySpan<byte> span, out int length)
    {
        length = 2;
        return BitConverter.ToInt16(span);
    }

    public static void WriteUnsignedShort(this Span<byte> span, ushort value, out int length)
    {
        length = 2;
        BitConverter.TryWriteBytes(span, value);
    }

    public static ushort ReadUnsignedShort(this ReadOnlySpan<byte> span, out int length)
    {
        length = 2;
        return BitConverter.ToUInt16(span);
    }

    public static void WriteInt(this Span<byte> span, int value, out int length)
    {
        length = 4;
        BitConverter.TryWriteBytes(span, value);
    }

    public static int ReadInt(this ReadOnlySpan<byte> span, out int length)
    {
        length = 4;
        return BitConverter.ToInt32(span);
    }

    public static void WriteUnsignedInt(this Span<byte> span, uint value, out int length)
    {
        length = 4;
        BitConverter.TryWriteBytes(span, value);
    }

    public static uint ReadUnsignedInt(this ReadOnlySpan<byte> span, out int length)
    {
        length = 4;
        return BitConverter.ToUInt32(span);
    }

    public static void WriteLong(this Span<byte> span, long value, out int length)
    {
        length = 8;
        BitConverter.TryWriteBytes(span, value);
    }

    public static long ReadLong(this ReadOnlySpan<byte> span, out int length)
    {
        length = 8;
        return BitConverter.ToInt64(span);
    }

    public static void WriteUnsignedLong(this Span<byte> span, ulong value, out int length)
    {
        length = 8;
        BitConverter.TryWriteBytes(span, value);
    }

    public static ulong ReadUnsignedLong(this ReadOnlySpan<byte> span, out int length)
    {
        length = 8;
        return BitConverter.ToUInt64(span);
    }

    public static void WriteFloat(this Span<byte> span, float value, out int length)
    {
        length = 4;
        BitConverter.TryWriteBytes(span, value);
    }

    public static float ReadFloat(this ReadOnlySpan<byte> span, out int length)
    {
        length = 4;
        return BitConverter.ToSingle(span);
    }

    public static void WriteDouble(this Span<byte> span, double value, out int length)
    {
        length = 8;
        BitConverter.TryWriteBytes(span, value);
    }

    public static double ReadDouble(this ReadOnlySpan<byte> span, out int length)
    {
        length = 8;
        return BitConverter.ToDouble(span);
    }

    public static void WriteCompactInteger(this Span<byte> span, long value, out int length)
    {
        int i = 0;
        do
        {
            byte part = (byte)value;
            bool hasMore = value > 0b_0111_1111;
            part.SetBit(7, hasMore);
            span[i] = part;
            i++;

            if (!hasMore)
            {
                break;
            }
        }
        while (true);

        length = i;
    }

    public static long ReadCompactInteger(this ReadOnlySpan<byte> span, out int length)
    {
        long value = 0;
        int i = 0;
        do
        {
            long part = span[i];
            bool hasMore = part.GetBit(7);
            part = part.ClearBit(7);
            value |= part << (i * 7);
            i++;

            if (!hasMore)
            {
                break;
            }
        }
        while (true);

        length = i;
        return value;
    }

    public static void WriteDateTime(this Span<byte> span, DateTime value, out int length)
        => span.WriteCompactInteger(((DateTimeOffset)value.ToUniversalTime()).ToUnixTimeSeconds(), out length);

    public static DateTime ReadDateTime(this ReadOnlySpan<byte> span, out int length)
        => DateTimeOffset.FromUnixTimeSeconds(span.ReadCompactInteger(out length)).LocalDateTime;

    public static void WriteBlock(this Span<byte> span, ReadOnlySpan<byte> source, out int length)
    {
        length = 0;

        span.WriteCompactInteger(source.Length, out int written);
        length += written;

        span[length..].WriteBytes(source, out written);
        length += written;
    }

    public static void ReadBlock(this ReadOnlySpan<byte> span, IDynamicBuffer destination, out int length)
    {
        length = 0;

        int dataLength = (int)span.ReadCompactInteger(out int read);
        length += read;

        destination.FillFrom(span.Slice(length, dataLength));
        length += dataLength;
    }

    public static byte[] ReadBlock(this ReadOnlySpan<byte> span, out int length)
    {
        length = 0;

        int dataLength = (int)span.ReadCompactInteger(out int read);
        length += read;

        byte[] data = new byte[dataLength];
        span.Slice(length, dataLength).CopyTo(data);
        length += dataLength;

        return data;
    }

    public static void WriteString(this Span<byte> span, string value, out int length)
        => span.WriteBlock(Encoding.UTF8.GetBytes(value), out length);

    public static string ReadString(this ReadOnlySpan<byte> span, out int length)
        => Encoding.UTF8.GetString(span.ReadBlock(out length));
}