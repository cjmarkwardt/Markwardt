namespace Markwardt;

public abstract class DataTupleSerializer<T> : DataSerializer<T>
{
    protected override T Read(string data)
        => ReadTuple(data.Split('~'));

    protected override string? Write(T value)
    {
        IEnumerable<object>? tuple = WriteTuple(value);
        return tuple is null ? null : string.Join('~', tuple);
    }

    protected abstract T ReadTuple(IReadOnlyList<string> data);
    protected abstract IEnumerable<object>? WriteTuple(T value);
}