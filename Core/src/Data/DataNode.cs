namespace Markwardt;

public record DataNode(string? Type, DataKind Kind, object Value)
{
    public T GetValue<T>()
        => (T)Value;
}