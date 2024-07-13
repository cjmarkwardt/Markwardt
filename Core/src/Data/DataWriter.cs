namespace Markwardt;

public interface IDataWriter
{
    void WriteData(string? type, byte[] value);
    void WriteByte(string? type, byte value);
    void WriteInteger(string? type, long value);
    void WriteNumber(string? type, float value);
    void WritePreciseNumber(string? type, double value);
    void WriteText(string? type, string value);
    void WriteItemCount(string? type, int value);
    void WritePairCount(string? type, int value);
}

public static class DataWriterExtensions
{
    public static void WriteList(this IDataWriter writer, string? type, int length, Action<IDataWriter, int> writeItem)
    {
        writer.WriteItemCount(type, length);
        for (int i = 0; i < length; i++)
        {
            writeItem(writer, i);
        }
    }

    public static void WriteList<T>(this IDataWriter writer, string? type, IEnumerable<T> items, Action<IDataWriter, T> writeItem)
    {
        writer.WriteItemCount(type, items.Count());
        foreach (T item in items)
        {
            writeItem(writer, item);
        }
    }

    public static void WritePair(this IDataWriter writer, string? type, Action<IDataWriter> writeKey, Action<IDataWriter> writeValue)
    {
        writer.WriteItemCount(type, 2);
        writeKey(writer);
        writeValue(writer);
    }
}

public class DataWriter(Stream stream) : IDataWriter
{
    public void WriteData(string? type, byte[] value)
    {
        WriteHeader(type, DataKind.Data);
        WriteBlock(value);
    }

    public void WriteByte(string? type, byte value)
    {
        WriteHeader(type, DataKind.Byte);
        stream.WriteByte(value);
    }

    public void WriteInteger(string? type, long value)
    {
        WriteHeader(type, DataKind.Integer);
        WriteCompactInteger(value);
    }

    public void WriteNumber(string? type, float value)
    {
        WriteHeader(type, DataKind.Number);
        stream.Write(BitConverter.GetBytes(value));
    }

    public void WritePreciseNumber(string? type, double value)
    {
        WriteHeader(type, DataKind.PreciseNumber);
        stream.Write(BitConverter.GetBytes(value));
    }

    public void WriteText(string? type, string value)
    {
        WriteHeader(type, DataKind.Text);
        WriteString(value);
    }

    public void WriteItemCount(string? type, int value)
    {
        WriteHeader(type, DataKind.ItemCount);
        WriteCompactInteger(value);
    }

    public void WritePairCount(string? type, int value)
    {
        WriteHeader(type, DataKind.PairCount);
        WriteCompactInteger(value);
    }

    private void WriteCompactInteger(long value)
    {
        List<byte> data = new(8);
        for (int i = 0; i < 8; i++)
        {
            byte datum = (byte)(value >> (i * 8));
            if (datum == 0)
            {
                break;
            }

            data.Add(datum);
        }

        stream.WriteByte((byte)data.Count);
        data.ForEach(stream.WriteByte);
    }

    private void WriteBlock(byte[]? value)
    {
        int length = value?.Length ?? 0;
        WriteCompactInteger(length);
        if (length > 0)
        {
            stream.Write(value);
        }
    }

    private void WriteString(string? value)
        => WriteBlock(value is null ? null : Encoding.UTF8.GetBytes(value));
    
    private void WriteHeader(string? type, DataKind kind)
    {
        WriteString(type);
        stream.WriteByte((byte)kind);
    }
}