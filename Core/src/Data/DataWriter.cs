namespace Markwardt;

public interface IDataWriter
{
    void WriteNull();
    void WriteBoolean(bool? value);
    void WriteInteger(BigInteger? value);
    void WriteSingle(float? value);
    void WriteDouble(double? value);
    void WriteBlock(ReadOnlySpan<byte> value);
    void WriteBlock(Action<IDynamicBuffer> write);
    void WriteSequence();
    void WriteObject(string? type = null);
    void WriteProperty(string name);
    void WriteStop();

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