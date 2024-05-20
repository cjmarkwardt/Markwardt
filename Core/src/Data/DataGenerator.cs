namespace Markwardt;

public interface IDataGenerator
{
    IDataSerializer? Generate(Type type);
}

public class DataGenerator<T, TSerializer> : IDataGenerator
    where TSerializer : IDataSerializer, new()
{
    public IDataSerializer? Generate(Type type)
    {
        if (type == typeof(T) || Nullable.GetUnderlyingType(type) == typeof(T))
        {
            return new TSerializer();
        }

        return null;
    }
}