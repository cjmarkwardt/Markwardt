namespace Markwardt;

public class Vector2DataSerializer : DataSubSerializer<Vector2>
{
    protected override void Serialize(IDataWriter writer, Vector2 value, IDataSerializer rootSerializer)
    {
        writer.WriteSingle(value.X);
        writer.WriteSingle(value.Y);
    }

    protected override Vector2 Deserialize(IDataReader reader, IDataSerializer rootSerializer)
        => new(reader.ReadSingle().ValueNotNull(), reader.ReadSingle().ValueNotNull());
}