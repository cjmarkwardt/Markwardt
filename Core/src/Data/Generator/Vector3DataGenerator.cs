namespace Markwardt;

public class Vector3DataGenerator : DataGenerator<Vector3, Vector3DataGenerator.Serializer>
{
    public class Serializer : DataTupleSerializer<Vector3?>
    {
        protected override Vector3? ReadTuple(IReadOnlyList<string> data)
            => new(float.Parse(data[0]), float.Parse(data[1]), float.Parse(data[2]));

        protected override IEnumerable<object>? WriteTuple(Vector3? value)
            => value is null ? null : [value.Value.X, value.Value.Y, value.Value.Z];
    }
}