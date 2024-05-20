namespace Markwardt;

public class DateTimeDataGenerator : DataGenerator<DateTime, DateTimeDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<DateTime?>
    {
        protected override DateTime? Read(string data)
            => new DateTime(long.Parse(data), DateTimeKind.Utc).ToLocalTime();

        protected override string? Write(DateTime? value)
            => value?.ToUniversalTime().Ticks.ToString();
    }
}