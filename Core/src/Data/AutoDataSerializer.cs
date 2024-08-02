namespace Markwardt;

public class AutoDataSerializer : IDataSerializer
{
    

    public void Serialize(ITypeDescription target, IDataWriter writer, object? value, IDataSerializer? rootSerializer = null)
    {
        
    }

    public object? Deserialize(ITypeDescription target, IDataReader reader, IDataSerializer? rootSerializer = null)
    {
        throw new NotImplementedException();
    }
}