namespace Markwardt;

public interface IPair<out TKey, out T> : IMultiDisposable
{
    TKey Key { get; }
    T Value { get; }
}

public static class PairExtensions
{
    public static IPair<TKey, T> AsPair<TKey, T>(this T obj, TKey key)
        => new Pair<TKey, T>(key, obj);
}

public readonly record struct Pair<TKey, T>(TKey Key, T Value) : IPair<TKey, T>
{
    public readonly void Dispose()
    {
        Key.TryDispose();
        Value.TryDispose();
    }

    public readonly async ValueTask DisposeAsync()
        => await Task.WhenAll(Key.TryDisposeAsync().AsTask(), Value.TryDisposeAsync().AsTask());
}