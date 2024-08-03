namespace Markwardt;

public interface IDataSubSerializer
{
    void Serialize(ITypeDescription target, IDataWriter writer, object value, IDataSerializer rootSerializer);
    object Deserialize(ITypeDescription target, IDataReader reader, IDataSerializer rootSerializer);
}

public abstract class DataSubSerializer<T> : IDataSubSerializer
    where T : notnull
{
    public void Serialize(ITypeDescription target, IDataWriter writer, object value, IDataSerializer rootSerializer)
    {
        Serialize(writer, (T)value, rootSerializer);
    }

    public object Deserialize(ITypeDescription target, IDataReader reader, IDataSerializer rootSerializer)
        => Deserialize(reader, rootSerializer);

    protected abstract void Serialize(IDataWriter writer, T value, IDataSerializer rootSerializer);
    protected abstract T Deserialize(IDataReader reader, IDataSerializer rootSerializer);
}