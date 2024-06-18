namespace Markwardt;

public interface IAccessManager : IAccessor
{

}

public class AccessManager : IAccessManager
{
    public ValueTask<IEnumerable<KeyValuePair<string, TResult>>> Access<T, TResult>(IEnumerable<string> ids, Func<T, ValueTask<TResult>> access)
    {
        throw new NotImplementedException();
    }

    public IObservable Watch(string id)
    {
        throw new NotImplementedException();
    }
}