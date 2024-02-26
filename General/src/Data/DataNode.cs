namespace Markwardt;

public class DataNode
{
    public static implicit operator DataNode(string? value)
        => new DataValue(value);

    public static implicit operator DataNode(char? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(bool? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(byte? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(sbyte? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(short? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(ushort? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(int? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(uint? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(long? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(ulong? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(float? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(double? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(decimal? value)
        => new DataValue(value?.ToString());

    public static implicit operator DataNode(DateTime? value)
        => new DataValue(value?.ToString(CultureInfo.InvariantCulture));

    public string? Type { get; set; }

    public DataNode AsNode()
        => this;

    public virtual Option<DataValue> AsValue()
        => Option.None<DataValue>();

    public virtual Option<string> AsText()
        => Option.None<string>();

    public virtual Option<char> AsCharacter()
        => Option.None<char>();

    public virtual Option<bool> AsBoolean()
        => Option.None<bool>();

    public virtual Option<byte> AsByte()
        => Option.None<byte>();

    public virtual Option<sbyte> AsSignedByte()
        => Option.None<sbyte>();

    public virtual Option<short> AsShort()
        => Option.None<short>();

    public virtual Option<ushort> AsUnsignedShort()
        => Option.None<ushort>();

    public virtual Option<int> AsInteger()
        => Option.None<int>();

    public virtual Option<uint> AsUnsignedInteger()
        => Option.None<uint>();

    public virtual Option<long> AsLong()
        => Option.None<long>();

    public virtual Option<ulong> AsUnsignedLong()
        => Option.None<ulong>();

    public virtual Option<float> AsFloat()
        => Option.None<float>();

    public virtual Option<double> AsDouble()
        => Option.None<double>();

    public virtual Option<decimal> AsDecimal()
        => Option.None<decimal>();

    public virtual Option<DateTime> AsDate()
        => Option.None<DateTime>();

    public virtual Option<DataObject> AsObject()
        => Option.None<DataObject>();
}