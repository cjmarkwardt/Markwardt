namespace Markwardt;

public static class TypeExtensions
{
    public static bool IsDelegate(this Type type)
        => typeof(MulticastDelegate).IsAssignableFrom(type);
        
    public static bool HasGenericTypeDefinition(this Type type, Type genericTypeDefinition)
        => type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition;

    public static bool IsAssignableTo(this Type type, Type c)
        => c.IsAssignableFrom(type);
        
    public static bool IsInstantiable(this Type type)
        => !type.IsInterface && !type.IsAbstract && !type.IsDelegate();

    public static Type? TryGetGenericTypeDefinition(this Type type)
        => type.IsGenericType ? type.GetGenericTypeDefinition() : null;

    public static Type[]? TryGetGenericArguments(this Type type)
        => type.IsGenericType ? type.GetGenericArguments() : null;

    public static string GetNonInterfaceName(this Type type)
        => type.IsInterface ? type.Name[1..] : type.Name;

    public static IEnumerable<MemberInfo> GetAllInterfaceMembers(this Type type)
    {
        foreach (MemberInfo member in type.GetMembers())
        {
            yield return member;
        }

        foreach (Type implementation in type.GetInterfaces())
        {
            foreach (MemberInfo member in implementation.GetMembers())
            {
                yield return member;
            }
        }
    }
}