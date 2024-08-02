namespace Markwardt;

public class Vector3DataSerializer : DataSubSerializer<Vector3>
{
    protected override void Serialize(IDataWriter writer, Vector3 value, IDataSerializer rootSerializer)
    {
        writer.WriteSingle(value.X);
        writer.WriteSingle(value.Y);
        writer.WriteSingle(value.Z);
    }

    protected override Vector3 Deserialize(IDataReader reader, IDataSerializer rootSerializer)
        => new(reader.ReadSingle().ValueNotNull(), reader.ReadSingle().ValueNotNull(), reader.ReadSingle().ValueNotNull());
}