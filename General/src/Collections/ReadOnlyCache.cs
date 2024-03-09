namespace Markwardt;

public interface IReadOnlyCache<in TKey, out T> : IReadOnlyList<T>, IKeyLookup<TKey, T>;