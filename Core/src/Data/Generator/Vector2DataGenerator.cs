namespace Markwardt;

public class Vector2DataGenerator : DataGenerator<Vector2, Vector2DataGenerator.Serializer>
{
    public class Serializer : DataTupleSerializer<Vector2?>
    {
        protected override Vector2? ReadTuple(IReadOnlyList<string> data)
            => new(float.Parse(data[0]), float.Parse(data[1]));

        protected override IEnumerable<object>? WriteTuple(Vector2? value)
            => value is null ? null : [value.Value.X, value.Value.Y];
    }
}