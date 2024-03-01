namespace Markwardt;

public interface IDataAccessor
{
    IMaybe<object?> GetProperty(string name);
    bool SetProperty(string name, object? value);
}