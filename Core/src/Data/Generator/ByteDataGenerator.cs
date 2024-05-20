namespace Markwardt;

public class ByteDataGenerator : DataGenerator<byte, ByteDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<byte?>
    {
        protected override byte? Read(string data)
            => byte.Parse(data);
    }
}