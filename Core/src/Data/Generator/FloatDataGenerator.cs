namespace Markwardt;

public class FloatDataGenerator : DataGenerator<float, FloatDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<float?>
    {
        protected override float? Read(string data)
            => float.Parse(data);
    }
}