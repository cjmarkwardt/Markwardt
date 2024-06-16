namespace Markwardt;

public interface IAssetReader
{
    ValueTask<object> Read(AssetId id, Stream data);
}