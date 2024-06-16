namespace Markwardt;

public interface IKeyLoader<TKey, T>
{
    IAsyncEnumerable<KeyValuePair<TKey, T>> Load(IEnumerable<TKey> keys);
}

public class KeyLoader<TKey, T>(Func<IEnumerable<TKey>, IAsyncEnumerable<KeyValuePair<TKey, T>>> @delegate) : IKeyLoader<TKey, T>
{
    public IAsyncEnumerable<KeyValuePair<TKey, T>> Load(IEnumerable<TKey> keys)
        => @delegate(keys);
}