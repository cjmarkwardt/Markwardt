namespace Markwardt;

[Singleton<DataSegmentTyper>]
public interface IDataSegmentTyper
{
    DataDictionary Create(Type type);
    string GetName(Type type);
    Type GetType(IDataNode node);
    bool Is(IDataNode node, Type type);
}

public class DataSegmentTyper : IDataSegmentTyper
{
    private readonly Dictionary<string, Type> types = [];

    public required ITypeSource Types { get; init; }

    public DataDictionary Create(Type type)
        => new() { Type = GetName(type) };

    public string GetName(Type type)
        => GetTypeName(type) ?? throw new InvalidOperationException($"Type {type} is not a data segment");

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
        foreach (Type type in Types.GetTypes().Where(x => x.IsInterface))
        {
            string? name = GetTypeName(type);
            if (name is not null)
            {
                types[name] = type;
            }
        }
    }

    private string? GetTypeName(Type type)
    {
        SegmentAttribute? attribute = type.GetCustomAttribute<SegmentAttribute>();
        if (attribute is not null)
        {
            if (attribute.Name is not null)
            {
                return attribute.Name;
            }
            else
            {
                string name = type.Name[1..];

                Type? parent = type.DeclaringType;
                while (parent is not null)
                {
                    name = parent.Name[1..];
                    parent = parent.DeclaringType;
                }

                return name;
            }
        }
        else
        {
            return null;
        }
    }
}