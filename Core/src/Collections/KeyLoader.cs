namespace Markwardt;

public delegate IDictionary<TKey, T> KeyLoader<TKey, T>(IEnumerable<TKey> keys);