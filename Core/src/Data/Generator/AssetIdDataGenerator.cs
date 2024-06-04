namespace Markwardt;

public class AssetIdDataGenerator : DataGenerator<AssetId, AssetIdDataGenerator.Serializer>
{
    public class Serializer : DataTupleSerializer<AssetId?>
    {
        protected override AssetId? ReadTuple(IReadOnlyList<string> data)
            => new(data[0], data[1]);

        protected override IEnumerable<object>? WriteTuple(AssetId? value)
            => value is null ? null : [value.Module, value.Value];
    }
}