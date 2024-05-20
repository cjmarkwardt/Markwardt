namespace Markwardt;

public class DecimalDataGenerator : DataGenerator<decimal, DecimalDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<decimal?>
    {
        protected override decimal? Read(string data)
            => decimal.Parse(data);
    }
}