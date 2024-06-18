namespace Markwardt;

public interface IAccessStore : IAccessLoader
{
    ValueTask<IEnumerable<KeyValuePair<string, T>>> Load<T>(IEnumerable<string> ids);
}