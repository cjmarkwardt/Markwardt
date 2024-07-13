namespace Markwardt;

public interface IDataSerializer
{
    void Serialize(IDataWriter writer, object value);
    object Deserialize(IDataReader reader);
}

public class TypeDataSerializer(Type type) : IDataSerializer
{
    public void Serialize(IDataWriter writer, object value)
    {
        if (value is DataSegment segment)
        {
            segment.Serialize(writer);
        }
    }

    public object Deserialize(IDataReader reader)
    {
        throw new NotImplementedException();
    }
}