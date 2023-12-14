namespace Markwardt;

public interface IDataAccessor
{
    Option<object?> GetProperty(string name);
    bool SetProperty(string name, object? value);
}