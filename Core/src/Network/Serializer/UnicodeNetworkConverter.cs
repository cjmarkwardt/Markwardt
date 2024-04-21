namespace Markwardt;

[Factory<UnicodeNetworkConverter>]
public delegate ValueTask<ITwoWayConverter<object, ReadOnlyMemory<byte>>> UnicodeNetworkConverterFactory();

public class UnicodeNetworkConverter : ITwoWayConverter<object, ReadOnlyMemory<byte>>
{
    public ReadOnlyMemory<byte> Convert(object target)
        => Encoding.UTF8.GetBytes(target.ToString() ?? "NULL");

    public object ConvertBack(ReadOnlyMemory<byte> data)
        => Encoding.UTF8.GetString(data.Span);
}