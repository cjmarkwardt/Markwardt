namespace Markwardt;

public class TimeSpanDataGenerator : DataGenerator<TimeSpan, TimeSpanDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<TimeSpan?>
    {
        protected override TimeSpan? Read(string data)
            => new(long.Parse(data));

        protected override string? Write(TimeSpan? value)
            => value?.Ticks.ToString();
    }
}