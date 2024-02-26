namespace Markwardt.Godot;

public abstract class CachedLoader : ILoader
{
    private readonly Dictionary<Key, object> cache = [];

    public async ValueTask<Failable<object>> TryLoad(string path, Type? type = null)
    {
        if (cache.TryGetValue(new Key(path, type), out object? instance))
        {
            return instance;
        }

        Failable<object> tryLoad = await AttemptLoad(path, type);
        if (tryLoad.Exception != null)
        {
            return tryLoad.Exception;
        }

        return tryLoad.Result;
    }

    public void ClearCache()
        => cache.Clear();

    protected void SetCache(string path, Type? type, object instance)
        => cache[new Key(path, type)] = instance;

    protected abstract ValueTask<Failable<object>> AttemptLoad(string path, Type? type = null);

    private sealed record Key(string Path, Type? Type);
}