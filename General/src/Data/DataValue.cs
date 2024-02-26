namespace Markwardt;

public class DataValue : DataNode
{
    public DataValue(string? content = null, string? type = null)
    {
        Content = content;
        Type = type;
    }

    public string? Content { get; set; }

    public override Option<DataValue> AsValue()
        => this.Some();

    public override Option<string> AsText()
        => Content != null ? Content.Some() : Option.None<string>();

    public override Option<char> AsCharacter()
        => Content?.Length == 1 ? Content[0].Some() : Option.None<char>();

    public override Option<bool> AsBoolean()
        => bool.TryParse(Content, out bool value) ? value.Some() : Option.None<bool>();

    public override Option<byte> AsByte()
        => byte.TryParse(Content, out byte value) ? value.Some() : Option.None<byte>();

    public override Option<sbyte> AsSignedByte()
        => sbyte.TryParse(Content, out sbyte value) ? value.Some() : Option.None<sbyte>();

    public override Option<short> AsShort()
        => short.TryParse(Content, out short value) ? value.Some() : Option.None<short>();

    public override Option<ushort> AsUnsignedShort()
        => ushort.TryParse(Content, out ushort value) ? value.Some() : Option.None<ushort>();

    public override Option<int> AsInteger()
        => int.TryParse(Content, out int value) ? value.Some() : Option.None<int>();

    public override Option<uint> AsUnsignedInteger()
        => uint.TryParse(Content, out uint value) ? value.Some() : Option.None<uint>();

    public override Option<long> AsLong()
        => long.TryParse(Content, out long value) ? value.Some() : Option.None<long>();

    public override Option<ulong> AsUnsignedLong()
        => ulong.TryParse(Content, out ulong value) ? value.Some() : Option.None<ulong>();

    public override Option<float> AsFloat()
        => float.TryParse(Content, out float value) ? value.Some() : Option.None<float>();

    public override Option<double> AsDouble()
        => double.TryParse(Content, out double value) ? value.Some() : Option.None<double>();

    public override Option<decimal> AsDecimal()
        => decimal.TryParse(Content, out decimal value) ? value.Some() : Option.None<decimal>();

    public override Option<DateTime> AsDate()
        => DateTime.TryParse(Content, CultureInfo.InvariantCulture, out DateTime value) ? value.Some() : Option.None<DateTime>();
}