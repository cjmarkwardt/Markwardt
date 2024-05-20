namespace Markwardt;

public class IntDataGenerator : DataGenerator<int, IntDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<int?>
    {
        protected override int? Read(string data)
            => int.Parse(data);
    }
}