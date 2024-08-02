namespace Markwardt;

public class StringDataSerializer : DataSubSerializer<string>
{
    protected override void Serialize(IDataWriter writer, string value, IDataSerializer rootSerializer)
        => writer.WriteBlock(x =>
        {
            x.Length = Encoding.UTF8.GetByteCount(value);
            Encoding.UTF8.GetBytes(value, x.Data.Span);
        });

    protected override string Deserialize(IDataReader reader, IDataSerializer rootSerializer)
        => Encoding.UTF8.GetString(reader.ReadBlock().ValueNotNull().Span);
}