namespace Markwardt;

[Singleton<TypeSource>]
public interface ITypeSource
{
    IEnumerable<Type> GetTypes();
}

public class TypeSource : ITypeSource
{
    public IEnumerable<Type> GetTypes()
        => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
}

public class TypeSetSource(IEnumerable<Type> types) : ITypeSource
{
    public IEnumerable<Type> GetTypes()
        => types;
}