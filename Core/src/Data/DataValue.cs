namespace Markwardt;

public record struct DataValue(string Content) : IDataNode
{
    public readonly bool PopChanges() => false;
}