namespace Markwardt;

public class DoubleDataGenerator : DataGenerator<double, DoubleDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<double?>
    {
        protected override double? Read(string data)
            => double.Parse(data);
    }
}