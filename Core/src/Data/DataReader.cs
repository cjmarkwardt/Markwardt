namespace Markwardt;

public interface IDataReader
{
    DataNode? Read(Func<DataNode, bool>? validate = null);
}

public static class DataReaderExtensions
{
    public static DataNode? Read(this IDataReader reader, DataKind kind, Func<DataNode, bool>? validate = null)
        => reader.Read(x => x.Kind == kind && (validate is null || validate(x)));

    public static (string? Type, IList<T> Items)? ReadList<T>(this IDataReader reader, Func<IDataReader, int, T> readItem)
    {
        DataNode? node = reader.Read(DataKind.ItemCount);
        if (node is null)
        {
            return null;
        }

        List<T> items = new(node.GetValue<int>());
        for (int i = 0; i < node.GetValue<int>(); i++)
        {
            items[i] = readItem(reader, i);
        }

        return (node.Type, items);
    }

    public static KeyValuePair<TKey, T>? ReadPair<TKey, T>(this IDataReader reader, Func<IDataReader, TKey> readKey, Func<IDataReader, T> readValue)
    {
        DataNode? node = reader.Read(DataKind.ItemCount, x => x.GetValue<int>() == 2);
        if (node is null)
        {
            return null;
        }

        return new KeyValuePair<TKey, T>(readKey(reader), readValue(reader));
    }
}

public class DataReader(Stream stream) : IDataReader
{
    public DataNode? Read(Func<DataNode, bool>? validate = null)
    {
        int rewind = 0;

        string? type = ReadString(ref rewind);

        DataKind kind = (DataKind)stream.ReadByte();
        rewind++;

        object value = kind switch
        {
            DataKind.Data => ReadBlock(ref rewind) ?? [],
            DataKind.Byte => ReadBytes(1, ref rewind)[0],
            DataKind.Integer => ReadCompactInteger(ref rewind),
            DataKind.Number => BitConverter.ToSingle(ReadBytes(4, ref rewind)),
            DataKind.PreciseNumber => BitConverter.ToDouble(ReadBytes(8, ref rewind)),
            DataKind.Text => ReadString(ref rewind) ?? string.Empty,
            DataKind.ItemCount => ReadCompactInteger(ref rewind),
            DataKind.PairCount => ReadCompactInteger(ref rewind),
            _ => throw new InvalidOperationException()
        };

        DataNode node = new(type, kind, value);
        if (validate is null || validate(node))
        {
            return node;
        }
        else
        {
            stream.Position -= rewind;
            return null;
        }
    }

    private long ReadCompactInteger(ref int rewind)
    {
        int length = stream.ReadByte();
        rewind++;
        
        byte[] bytes = ReadBytes(length, ref rewind);

        long data = 0;
        for (int i = 0; i < length; i++)
        {
            data |= (long)bytes[i] << (i * 8);
        }

        return data;
    }

    private byte[]? ReadBlock(ref int rewind)
    {
        int length = (int)ReadCompactInteger(ref rewind);
        return length == 0 ? null : ReadBytes(length, ref rewind);
    }

    private string? ReadString(ref int rewind)
    {
        byte[]? data = ReadBlock(ref rewind);
        return data is null ? null : Encoding.UTF8.GetString(data);
    }

    private byte[] ReadBytes(int length, ref int rewind)
    {
        byte[] data = new byte[length];
        rewind += length;
        stream.Read(data);
        return data;
    }
}