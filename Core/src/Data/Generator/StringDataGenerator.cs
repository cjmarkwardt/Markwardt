namespace Markwardt;

public class StringDataGenerator : DataGenerator<string, StringDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<string?>
    {
        protected override string? Read(string data)
            => data;
    }
}