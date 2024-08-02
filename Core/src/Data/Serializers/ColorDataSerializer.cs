namespace Markwardt;

public class ColorDataSerializer : DataSubSerializer<Color>
{
    protected override void Serialize(IDataWriter writer, Color value, IDataSerializer rootSerializer)
    {
        writer.WriteInteger(value.R);
        writer.WriteInteger(value.G);
        writer.WriteInteger(value.B);
        writer.WriteInteger(value.A);
    }

    protected override Color Deserialize(IDataReader reader, IDataSerializer rootSerializer)
        => Color.FromArgb(reader.ReadInt().ValueNotNull(), reader.ReadInt().ValueNotNull(), reader.ReadInt().ValueNotNull(), reader.ReadInt().ValueNotNull());
}