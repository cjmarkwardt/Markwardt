namespace Markwardt;

public interface IMap<TKey, T> : IReadOnlyMap<TKey, T>, IKeyRemover<TKey>, IManyList<IPair<TKey, T>>;