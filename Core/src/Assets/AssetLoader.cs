namespace Markwardt;

public interface IAssetLoader
{
    IEnumerable<string> FilterHandled(IEnumerable<string> keys);
    IAsyncEnumerable<KeyValuePair<string, Failable<object>>> Load(IEnumerable<string> keys);
}