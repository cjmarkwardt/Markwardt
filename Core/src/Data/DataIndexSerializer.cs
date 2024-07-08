namespace Markwardt;

public interface IDataIndexSerializer
{
    ValueTask<IDictionary<string, int>> Deserialize(IDataPointer source);
    ValueTask Serialize(IDataPointer destination, IReadOnlyDictionary<string, int> index);
}