namespace Markwardt;

public interface IDataWriter
{
    void WriteNull(string? type = null);
    void WriteBoolean(bool? value, string? type = null);
    void WriteInteger(BigInteger? value, string? type = null);
    void WriteSingle(float? value, string? type = null);
    void WriteDouble(double? value, string? type = null);
    void WriteString(string? value, string? type = null);
    void WriteBlock(ReadOnlySpan<byte> value, string? type = null);
    void WriteBlock(Action<IDynamicBuffer> write, string? type = null);
    void WriteSequence(string? type = null);
    void WritePairSequence(string? type = null);
    void WritePropertySequence(string? type = null);
    void WriteStopSequence();
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