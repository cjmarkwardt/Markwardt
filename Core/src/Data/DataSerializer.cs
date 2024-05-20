namespace Markwardt;

public interface IDataSerializer
{
    object? Deserialize(string? data);
    string? Serialize(object? value);
}

public abstract class DataSerializer<T> : IDataSerializer
{
    public object? Deserialize(string? data)
        => data is null ? null : Read(data);

    public string? Serialize(object? value)
        => Write((T)value!);

    protected abstract T Read(string data);

    protected virtual string? Write(T value)
        => value?.ToString();
}