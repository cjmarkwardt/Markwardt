namespace Markwardt;

public class Vector4DataGenerator : DataGenerator<Vector4, Vector4DataGenerator.Serializer>
{
    public class Serializer : DataTupleSerializer<Vector4?>
    {
        protected override Vector4? ReadTuple(IReadOnlyList<string> data)
            => new(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]), float.Parse(data[3]));

        protected override IEnumerable<object>? WriteTuple(Vector4? value)
            => value is null ? null : [value.Value.X, value.Value.Y, value.Value.Z, value.Value.W];
    }
}