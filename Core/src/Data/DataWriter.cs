namespace Markwardt;

public interface IDataWriter
{
    IDynamicBuffer Buffer { get; }

    Stream? Destination { get; set; }

    void WriteNull();
    void WriteBoolean(bool? value);
    void WriteInteger(BigInteger? value);
    void WriteSingle(float? value);
    void WriteDouble(double? value);
    void WriteString(string? value);
    void WriteBlock(ReadOnlySpan<byte> value);
    void WriteBlock();
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

public class DataWriter : IDataWriter
{
    private readonly DirectDataTransformer transformer = new();

    public Stream? Destination { get; set; }

    public void WriteNull()
        => WriteDirectKind(DataKind.Null);

    public void WriteBoolean(bool? value)
    {
        if (value is null)
        {
            WriteNull();
        }
        else
        {
            WriteDirectKind(value.Value ? DataKind.True : DataKind.False);
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
            WriteDirectKind(DataKind.Integer);
            WriteDirectInteger(value.Value);
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
            WriteDirectKind(DataKind.Single);
            WriteDirectSingle(value.Value);
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
            WriteDirectKind(DataKind.Double);
            WriteDirectDouble(value.Value);
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
            WriteDirectKind(DataKind.String);
            WriteDirectString(value);
        }
    }

    public void WriteBlock(ReadOnlySpan<byte> value)
    {
        WriteDirectKind(DataKind.Block);
        WriteDirectBlock(value);
    }

    public void WriteStart()
        => WriteDirectKind(DataKind.Start);

    public void WriteEnd()
        => WriteDirectKind(DataKind.End);

    public void WriteType(string name)
    {
        WriteDirectKind(DataKind.Type);
        WriteDirectString(name);
    }

    public void WriteProperty(string name)
    {
        WriteDirectKind(DataKind.Property);
        WriteDirectString(name);
    }

    private void WriteDirectKind(DataKind kind)
        => Destination?.WriteByte((byte)kind);

    private void WriteDirectBytes(ReadOnlySpan<byte> source)
        => Destination?.Write(source);

    private void WriteDirectSingle(float value)
    {
        if (Destination is not null)
        {
            BitConverter.TryWriteBytes(buffer.Span, value);
            Destination.Write(buffer.Span[..4]);
        }
    }

    private void WriteDirectDouble(double value)
    {
        if (Destination is not null)
        {
            BitConverter.TryWriteBytes(buffer.Span, value);
            Destination.Write(buffer.Span[..8]);
        }
    }

    private void WriteDirectInteger(BigInteger value)
    {
        if (Destination is not null)
        {
            value.TryWriteBytes(buffer.Span, out int length);
            Destination.WriteByte((byte)length);
            WriteDirectBytes(buffer.Span[..length]);
        }
    }

    private void WriteDirectBlock(ReadOnlySpan<byte> source)
    {
        WriteDirectInteger(source.Length);
        WriteDirectBytes(source);
    }

    private void WriteDirectString(string value)
        => WriteDirectBlock(Encoding.UTF8.GetBytes(value));
}