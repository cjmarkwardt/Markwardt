namespace Markwardt;

public class SequenceDataSerializer<T> : DataSubSerializer<T>
{
    protected override void Serialize(IDataWriter writer, T value, IDataSerializer rootSerializer)
    {
        writer.WriteSequence();
    }

    protected override T Deserialize(IDataReader reader, IDataSerializer rootSerializer)
    {
        throw new NotImplementedException();
    }
}
