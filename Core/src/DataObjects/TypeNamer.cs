namespace Markwardt;

[Transient]
public interface ITypeNamer
{
    string GetName(Type type);
    Type GetType(string name);
}

public class TypeNamer : ITypeNamer
{
    private readonly Dictionary<string, Type> types = [];
    private readonly Dictionary<Type, string> typeNames = [];

    public string GetName(Type type)
        => typeNames[type];

    public Type GetType(string name)
        => types[name];

    public void Register(Type type, string? name = null)
    {
        types.Add(name ?? type.Name, type);
        typeNames.Add(type, name ?? type.Name);
    }
}