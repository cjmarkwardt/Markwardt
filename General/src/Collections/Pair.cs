namespace Markwardt;

public interface IPair<out TKey, out T>
{
    TKey Key { get; }
    T Value { get; }
}

public static class PairExtensions
{
    public static IPair<TKey, T> AsPair<TKey, T>(this T obj, TKey key)
        => new Pair<TKey, T>(key, obj);
}

public record struct Pair<TKey, T>(TKey Key, T Value) : IPair<TKey, T>;