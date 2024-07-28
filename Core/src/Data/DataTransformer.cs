namespace Markwardt;

public interface IDataTransformer : IDataWriter, IDataReader
{
    Stream? Target { get; set; }
}

public interface IDataReader
{
    object? Read(IDynamicBuffer? buffer = null);
}

public interface IDataWriter
{
    void WriteNull();
    void WriteBoolean(bool? value);
    void WriteInteger(BigInteger? value);
    void WriteSingle(float? value);
    void WriteDouble(double? value);
    void WriteString(string? value);
    void WriteBlock(ReadOnlySpan<byte> value);
    void WriteBlock(Action<IDynamicBuffer> write);
    void WriteStart();
    void WriteEnd();
    void WriteType(string name);
    void WriteProperty(string name);

    void WriteBlock(ReadOnlyMemory<byte>? value)
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            WriteBlock(value.Value.Span);
        }
    }
}

public class DataTransformer : IDataTransformer
{
    private readonly DirectDataTransformer transformer = new();

    public Stream? Target { get; set; }

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

    public void WriteString(string? value)
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            transformer.WriteKind(DataKind.String);
            transformer.WriteString(value);
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

    public void WriteStart()
        => transformer.WriteKind(DataKind.Start);

    public void WriteEnd()
        => transformer.WriteKind(DataKind.End);

    public void WriteType(string name)
    {
        transformer.WriteKind(DataKind.Type);
        transformer.WriteString(name);
    }

    public void WriteProperty(string name)
    {
        transformer.WriteKind(DataKind.Property);
        transformer.WriteString(name);
    }

    public object? Read(IDynamicBuffer? buffer = null)
        => transformer.ReadKind() switch
        {
            DataKind.Null => null,
            DataKind.False => false,
            DataKind.True => true,
            DataKind.Integer => transformer.ReadInteger(),
            DataKind.Single => transformer.ReadSingle(),
            DataKind.Double => transformer.ReadDouble(),
            DataKind.String => transformer.ReadString(),
            DataKind.Block => transformer.ReadBlock(buffer),
            DataKind.Start => DataSignal.Start,
            DataKind.End => DataSignal.End,
            DataKind.Type => new DataTypeSignal(transformer.ReadString()),
            DataKind.Property => new DataPropertySignal(transformer.ReadString()),
            DataKind kind => throw new NotSupportedException(kind.ToString()),
        };
}