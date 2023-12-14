namespace Markwardt;

public interface IIdDataSource : IMultiDisposable
{
    ValueTask Save(IEnumerable<IdData> changedEntities, IEnumerable<string> deletedEntities, IEnumerable<KeyValuePair<string, string>> changedIndexes, IEnumerable<string> deletedIndexes);
    ValueTask<IdData?> TryLoadEntity(string id);
    ValueTask<string?> TryLoadIndex(string index);
}