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