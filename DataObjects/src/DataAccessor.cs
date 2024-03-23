namespace Markwardt;

public interface IDataAccessor
{
    Maybe<object?> GetProperty(string name);
    bool SetProperty(string name, object? value);
}