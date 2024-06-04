namespace Markwardt;

public class ColorDataGenerator : DataGenerator<Color, ColorDataGenerator.Serializer>
{
    public class Serializer : DataTupleSerializer<Color?>
    {
        protected override Color? ReadTuple(IReadOnlyList<string> data)
            => Color.FromArgb(int.Parse(data[0]), int.Parse(data[1]), int.Parse(data[2]), int.Parse(data[3]));

        protected override IEnumerable<object>? WriteTuple(Color? value)
            => value is null ? null : [value.Value.R, value.Value.G, value.Value.B, value.Value.A];
    }
}