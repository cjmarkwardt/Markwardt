namespace Markwardt;

public class EnumDataGenerator : IDataGenerator
{
    public IDataSerializer? Generate(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type.IsEnum)
        {
            return new Serializer(type);
        }

        return null;
    }

    public class Serializer(Type type) : DataSerializer<object?>
    {
        protected override object? Read(string data)
            => Enum.ToObject(type, long.Parse(data));

        protected override string? Write(object? value)
            => value is null ? null : Convert.ToInt64(value).ToString();
    }
}