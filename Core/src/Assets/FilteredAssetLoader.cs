namespace Markwardt;

public class FilteredAssetLoader(IAssetLoader Loader, Func<string, bool> IsHandled) : IAssetLoader
{
    public FilteredAssetLoader(IAssetLoader loader, string prefix)
        : this(loader, x => x.StartsWith(prefix)) { }

    public IEnumerable<string> FilterHandled(IEnumerable<string> keys)
        => keys.Where(IsHandled);

    public IAsyncEnumerable<KeyValuePair<string, Failable<object>>> Load(IEnumerable<string> keys)
        => Loader.Load(keys.Where(IsHandled));
}