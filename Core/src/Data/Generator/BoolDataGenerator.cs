namespace Markwardt;

public class BoolDataGenerator : DataGenerator<bool, BoolDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<bool?>
    {
        protected override bool? Read(string data)
            => data == "1";

        protected override string? Write(bool? value)
        {
            if (value is null)
            {
                return null;
            }
            else if (value is true)
            {
                return "1";
            }
            else
            {
                return "0";
            }
        }
    }
}