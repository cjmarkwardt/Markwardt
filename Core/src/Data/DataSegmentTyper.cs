namespace Markwardt;

[Singleton<DataSegmentTyper>]
public interface IDataSegmentTyper
{
    DataDictionary Create(Type type);
    Type GetType(IDataNode node);
    bool Is(IDataNode node, Type type);
}

public class DataSegmentTyper : IDataSegmentTyper
{
    private readonly Dictionary<string, Type> types = [];

    public required ITypeSource Types { get; init; }

    public DataDictionary Create(Type type)
        => new() { Type = type.GetCustomAttribute<SegmentAttribute>()?.Name ?? throw new InvalidOperationException($"Type {type} is not a data segment") };

    public Type GetType(IDataNode node)
    {
        string name = node?.AsDictionary()?.Type ?? throw new InvalidOperationException("Node is not a data segment");

        if (types.TryGetValue(name, out Type? type))
        {
            return type;
        }

        Refresh();

        return types[name];
    }

    public bool Is(IDataNode node, Type type)
        => GetType(node).IsAssignableTo(type);

    private void Refresh()
    {
        foreach (Type type in Types.GetTypes())
        {
            SegmentAttribute? attribute = type.GetCustomAttribute<SegmentAttribute>();
            if (attribute is not null)
            {
                types[attribute.Name] = type;
            }
        }
    }
}