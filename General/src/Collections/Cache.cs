namespace Markwardt;

public interface ICache<in TKey, T> : IReadOnlyCache<TKey, T>, IKeyRemover<TKey>, IManyList<T>;