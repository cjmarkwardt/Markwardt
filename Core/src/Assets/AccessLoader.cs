namespace Markwardt;

public interface IAccessLoader
{
    ValueTask<IEnumerable<KeyValuePair<string, T>>> Load<T>(IEnumerable<string> ids);
}