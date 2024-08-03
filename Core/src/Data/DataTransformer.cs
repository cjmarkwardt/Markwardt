namespace Markwardt;

public interface IDataTransformer : IDataWriter, IDataReader
{
    Stream? Target { get; set; }
}

public class DataTransformer : IDataTransformer
{
    private readonly DirectDataTransformer transformer = new();

    public Stream? Target { get => transformer.Target; set => transformer.Target = value; }

    public void WriteNull(string? type = null)
        => transformer.WriteKind(DataKind.Null, type);

    public void WriteBoolean(bool? value, string? type = null)
    {
        if (value is null)
        {
            WriteNull(type);
        }
        else
        {
            transformer.WriteKind(value.Value ? DataKind.True : DataKind.False, type);
        }
    }

    public void WriteInteger(BigInteger? value, string? type = null)
    {
        if (value is null)
        {
            WriteNull(type);
        }
        else
        {
            transformer.WriteKind(DataKind.Integer, type);
            transformer.WriteInteger(value.Value);
        }
    }

    public void WriteSingle(float? value, string? type = null)
    {
        if (value is null)
        {
            WriteNull(type);
        }
        else
        {
            transformer.WriteKind(DataKind.Single, type);
            transformer.WriteSingle(value.Value);
        }
    }

    public void WriteDouble(double? value, string? type = null)
    {
        if (value is null)
        {
            WriteNull(type);
        }
        else
        {
            transformer.WriteKind(DataKind.Double, type);
            transformer.WriteDouble(value.Value);
        }
    }

    public void WriteString(string? value, string? type = null)
    {
        if (value is null)
        {
            WriteNull(type);
        }
        else
        {
            transformer.WriteKind(DataKind.String, type);
            transformer.WriteString(value);
        }
    }

    public void WriteBlock(ReadOnlySpan<byte> value, string? type = null)
    {
        transformer.WriteKind(DataKind.Block, type);
        transformer.WriteBlock(value);
    }

    public void WriteBlock(Action<IDynamicBuffer> write, string? type = null)
    {
        transformer.WriteKind(DataKind.Block, type);
        transformer.WriteBlock(write);
    }

    public void WriteSequence(string? type = null)
        => transformer.WriteKind(DataKind.Sequence, type);

    public void WritePairSequence(string? type = null)
        => transformer.WriteKind(DataKind.PairSequence, type);

    public void WritePropertySequence(string? type = null)
        => transformer.WriteKind(DataKind.PropertySequence, type);

    public void WriteProperty(string name)
        => transformer.WriteString(name);

    public void WriteStopSequence()
        => transformer.WriteKind(DataKind.SequenceStop, null);

    public (object? Value, string? Type) Read(IDynamicBuffer? buffer = null)
    {
        (DataKind kind, string? type) = transformer.ReadKind();
        return kind switch
        {
            DataKind.Null => null,
            DataKind.False => false,
            DataKind.True => true,
            DataKind.Integer => transformer.ReadInteger(),
            DataKind.Single => transformer.ReadSingle(),
            DataKind.Double => transformer.ReadDouble(),
            DataKind.Block => transformer.ReadBlock(buffer),
            DataKind.Sequence => new DataSequenceSignal(),
            DataKind.PropertySequence => new DataObjectSignal(),
            DataKind.SequenceStop => new DataStopSignal(),
            DataKind.TypedObject => new DataObjectSignal(transformer.ReadString()),
            DataKind.PropertySequence => new DataObjectSignal(),
            DataKind.Type => new DataTypeSignal(transformer.ReadString()),
            DataKind.Property => new DataPropertySignal(transformer.ReadString()),
            DataKind.Items => DataSignal.Items,
            _ => throw new NotSupportedException(kind.ToString()),
        };
    }
}