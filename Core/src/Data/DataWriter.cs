namespace Markwardt;

public interface IDataTransformer : IDataWriter
{
    Stream? Target { get; set; }
}

public interface IDataBuffered
{
    IDynamicBuffer Buffer { get; }
}

public interface IDataReader : IDataBuffered
{
    object? Read(IDynamicBuffer? buffer = null);
}

public interface IDataWriter : IDataBuffered
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

    public IDynamicBuffer Buffer => transformer.Buffer;

    public void WriteNull()
        => transformer.WriteKind(DataKind.Null);

    public void WriteBoolean(bool? value)
        => WriteNullable(value, x => transformer.WriteKind(x ? DataKind.True : DataKind.False));

    public void WriteInteger(BigInteger? value)
        => WriteNullable(value, x =>
        {
            transformer.WriteKind(DataKind.Integer);
            transformer.WriteInteger(x);
        });

    public void WriteSingle(float? value)
        => WriteNullable(value, x =>
        {
            transformer.WriteKind(DataKind.Single);
            transformer.WriteSingle(x);
        });

    public void WriteDouble(double? value)
        => WriteNullable(value, x =>
        {
            transformer.WriteKind(DataKind.Double);
            transformer.WriteDouble(x);
        });

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

    public void WriteBufferBlock()
        => WriteBlock(Buffer.Data);

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

    private void WriteNullable<T>(T? value, DataKind kind, Action<T>? write)
        where T : struct
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            transformer.WriteKind(kind);
            write?.Invoke(value.Value);
        }
    }
}