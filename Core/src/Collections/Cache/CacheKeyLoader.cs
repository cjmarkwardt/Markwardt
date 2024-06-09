namespace Markwardt;

public interface ICacheKeyLoader<TKey, T>
{
    ValueTask<IDictionary<TKey, T>> Load(IEnumerable<TKey> keys);
}

public class CacheKeyLoader<TKey, T>(CacheKeyLoader<TKey, T>.Delegate @delegate) : ICacheKeyLoader<TKey, T>
{
    public delegate ValueTask<IDictionary<TKey, T>> Delegate(IEnumerable<TKey> keys);

    public async ValueTask<IDictionary<TKey, T>> Load(IEnumerable<TKey> keys)
        => await @delegate(keys);
}