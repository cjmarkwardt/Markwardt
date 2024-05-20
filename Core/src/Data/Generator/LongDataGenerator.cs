namespace Markwardt;

public class LongDataGenerator : DataGenerator<long, LongDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<long?>
    {
        protected override long? Read(string data)
            => long.Parse(data);
    }
}