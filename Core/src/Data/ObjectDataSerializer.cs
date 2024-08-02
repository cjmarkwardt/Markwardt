namespace Markwardt;

public class ObjectDataSerializer : IDataSerializer
{
    public ObjectDataSerializer(Type type)
    {
        foreach (PropertyInfo property in type.GetProperties())
        {

        }
    }

    public void Serialize(ITypeDescription target, IDataWriter writer, object? value, IDataSerializer? rootSerializer = null)
    {
        
    }

    public object? Deserialize(ITypeDescription target, IDataReader reader, IDataSerializer? rootSerializer = null)
    {
        throw new NotImplementedException();
    }
}