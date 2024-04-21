namespace Markwardt;

[AttributeUsage(AttributeTargets.Interface)]
public class NodeServiceAttribute(string? path = null) : Attribute
{
    public NodeServiceAttribute(Type parent, string path)
        : this(parent.GetCustomAttribute<NodeServiceAttribute>().NotNull().Path + "/" + path) { }

    public string? Path => path;
}

[AttributeUsage(AttributeTargets.Interface)]
public class NodeServiceAttribute<TParent>(string path) : NodeServiceAttribute(typeof(TParent), path);