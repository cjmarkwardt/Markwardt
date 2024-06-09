namespace Markwardt;

public interface ICacheKeyRequester<TRequest, TKey>
{
    ValueTask<IEnumerable<TKey>> Request(TRequest request);
}

public class CacheKeyRequester<TRequest, TKey>(CacheKeyRequester<TRequest, TKey>.Delegate @delegate) : ICacheKeyRequester<TRequest, TKey>
{
    public delegate ValueTask<IEnumerable<TKey>> Delegate(TRequest request);

    public async ValueTask<IEnumerable<TKey>> Request(TRequest request)
        => await @delegate(request);
}