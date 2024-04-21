namespace Markwardt;

public interface IDataRepository : IMultiDisposable, IDataIndexer
{
    void DeleteIndex(string index);
    ValueTask Clean();
    ValueTask Save();
}