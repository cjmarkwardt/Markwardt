namespace Markwardt;

public class DataObject : DataNode
{
    public DataObject(string? type = null)
    {
        Type = type;
    }

    public IList<DataNode> Items { get; } = [];
    public IDictionary<string, DataNode> Properties { get; } = new Dictionary<string, DataNode>();

    public override Option<DataObject> AsObject()
        => this.Some();
}