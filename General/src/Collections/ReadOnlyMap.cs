namespace Markwardt;

public interface IReadOnlyMap<TKey, out T> : IReadOnlyList<IPair<TKey, T>>, IKeyLookup<TKey, T>;