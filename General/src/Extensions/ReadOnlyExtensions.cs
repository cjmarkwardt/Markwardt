namespace Markwardt;

public static class ReadOnlyExtensions
{
    public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary)
        => new ReadOnlyDictionary<TKey, TValue>(dictionary);
}