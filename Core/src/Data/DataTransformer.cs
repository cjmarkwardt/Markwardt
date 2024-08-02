namespace Markwardt;

public interface IDataTransformer : IDataWriter, IDataReader
{
    Stream? Target { get; set; }
}

public class DataTransformer : IDataTransformer
{
    private readonly DirectDataTransformer transformer = new();

    public Stream? Target { get => transformer.Target; set => transformer.Target = value; }

    public void WriteNull()
        => transformer.WriteKind(DataKind.Null);

    public void WriteBoolean(bool? value)
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            transformer.WriteKind(value.Value ? DataKind.True : DataKind.False);
        }
    }

    public void WriteInteger(BigInteger? value)
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            transformer.WriteKind(DataKind.Integer);
            transformer.WriteInteger(value.Value);
        }
    }

    public void WriteSingle(float? value)
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            transformer.WriteKind(DataKind.Single);
            transformer.WriteSingle(value.Value);
        }
    }

    public void WriteDouble(double? value)
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            transformer.WriteKind(DataKind.Double);
            transformer.WriteDouble(value.Value);
        }
    }

    public void WriteBlock(ReadOnlySpan<byte> value)
    {
        transformer.WriteKind(DataKind.Block);
        transformer.WriteBlock(value);
    }

    public void WriteBlock(Action<IDynamicBuffer> write)
    {
        transformer.WriteKind(DataKind.Block);
        transformer.WriteBlock(write);
    }

    public void WriteSequence()
        => transformer.WriteKind(DataKind.Sequence);

    public void WriteObject(string? type = null)
    {
        transformer.WriteKind(type is null ? DataKind.PropertySequence : DataKind.TypedObject);
        
        if (type is not null)
        {
            transformer.WriteString(type);
        }
    }

    public void WriteProperty(string name)
        => transformer.WriteString(name);

    public void WriteStop()
        => transformer.WriteKind(DataKind.Stop);

    public object? Read(IDynamicBuffer? buffer = null)
        => transformer.ReadKind() switch
        {
            DataKind.Null => null,
            DataKind.False => false,
            DataKind.True => true,
            DataKind.Integer => transformer.ReadInteger(),
            DataKind.Single => transformer.ReadSingle(),
            DataKind.Double => transformer.ReadDouble(),
            DataKind.Block => transformer.ReadBlock(buffer),
            DataKind.Stop => new DataStopSignal(),
            DataKind.Sequence => new DataSequenceSignal(),
            DataKind.PropertySequence => new DataObjectSignal(),
            DataKind.TypedObject => new DataObjectSignal(transformer.ReadString()),
            DataKind.PropertySequence => new DataObjectSignal(),
            DataKind.Type => new DataTypeSignal(transformer.ReadString()),
            DataKind.Property => new DataPropertySignal(transformer.ReadString()),
            DataKind.Items => DataSignal.Items,
            DataKind kind => throw new NotSupportedException(kind.ToString()),
        };
}