namespace Markwardt;

public interface IAssetDataReader
{
    ValueTask<object> Read(string id, Stream data);
}