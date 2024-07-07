namespace Markwardt;

public class AssetIdDataGenerator : DataGenerator<AssetId, AssetIdDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<AssetId?>
    {
        protected override AssetId? Read(string data)
            => new(data);

        protected override string? Write(AssetId? value)
        {
            if (value is null)
            {
                return null;
            }
            else
            {
                return value.Value;
            }
        }
    }
}