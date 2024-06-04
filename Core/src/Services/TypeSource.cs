namespace Markwardt;

[Singleton<TypeSource>]
public interface ITypeSource
{
    IEnumerable<Type> GetAll();
}

public class TypeSource : ITypeSource
{
    public IEnumerable<Type> GetAll()
        => AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes());
}

public class TypeSetSource(IEnumerable<Type> types) : ITypeSource
{
    public IEnumerable<Type> GetAll()
        => types;
}