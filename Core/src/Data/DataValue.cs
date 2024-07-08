namespace Markwardt;

public record struct DataValue(string Content) : IDataNode
{
    public static implicit operator DataValue(string content)
        => new(content);

    public static implicit operator string(DataValue value)
        => value.Content;

    public readonly bool PopChanges() => false;
}