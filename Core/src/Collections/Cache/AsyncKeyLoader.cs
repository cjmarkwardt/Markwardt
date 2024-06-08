namespace Markwardt;

public delegate ValueTask<IDictionary<TKey, T>> AsyncKeyLoader<TKey, T>(IEnumerable<TKey> keys);