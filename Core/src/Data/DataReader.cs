namespace Markwardt;

public interface IDataReader
{
    Stream? Source { get; set; }

    object? Read(IDynamicBuffer? buffer = null);
}

public class DataReader : IDataReader
{
    public Stream? Source { get; set; }

    public object? Read(IDynamicBuffer? buffer = null)
    {
        if (Source is null)
        {
            return null;
        }

        DataKind kind = (DataKind)Source.ReadByte();

        switch (kind)
        {
            case DataKind.Null:
                return null;
            case DataKind.False:
                return false;
            case DataKind.True:
                return true;
            case DataKind.Integer:
                return ReadDirectInteger();
            case DataKind.Single:
                float singleValue = BitConverter.ToSingle(Source.Span);
                Move(4);
                return singleValue;
            case DataKind.Double:
                double doubleValue = BitConverter.ToDouble(Source.Span);
                Move(8);
                return doubleValue;
            case DataKind.String:
                return ReadDirectString();
            case DataKind.Block:
                int length = (int)ReadDirectInteger();
                buffer ??= new DynamicBuffer(length);
                buffer.FillFrom(Source.Span);
                Move(length);
                return buffer.Data;
            case DataKind.Start:
                return DataSignal.Start;
            case DataKind.End:
                return DataSignal.End;
            case DataKind.Type:
                return new DataTypeSignal(ReadDirectString());
            case DataKind.Property:
                return new DataPropertySignal(ReadDirectString());
            default:
                throw new NotSupportedException(kind.ToString());
        }
    }

    private BigInteger ReadDirectInteger()
    {
        int length = Source!.ReadByte();
        
    }

    private string ReadDirectString()
    {
        ReadOnlySpan<byte> target = Source.Span;

        int i = 0;
        while (target[i] != 0)
        {
            i++;
        }

        string value = i == 0 ? string.Empty : Encoding.UTF8.GetString(target[..i]);
        Move(i + 1);
        return value;
    }
}