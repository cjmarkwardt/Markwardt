namespace Markwardt;

public class Vector4DataSerializer : DataSubSerializer<Vector4>
{
    protected override void Serialize(IDataWriter writer, Vector4 value, IDataSerializer rootSerializer)
    {
        writer.WriteSingle(value.X);
        writer.WriteSingle(value.Y);
        writer.WriteSingle(value.Z);
        writer.WriteSingle(value.W);
    }

    protected override Vector4 Deserialize(IDataReader reader, IDataSerializer rootSerializer)
        => new(reader.ReadSingle().ValueNotNull(), reader.ReadSingle().ValueNotNull(), reader.ReadSingle().ValueNotNull(), reader.ReadSingle().ValueNotNull());
}