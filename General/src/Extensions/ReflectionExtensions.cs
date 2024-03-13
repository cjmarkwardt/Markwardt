namespace Markwardt;

public static class ReflectionExtensions
{
    public static bool HasCustomAttribute(this Type type, Type attributeType)
        => type.GetCustomAttribute(attributeType) is not null;

    public static bool HasCustomAttribute<TAttribute>(this Type type)
        where TAttribute : Attribute
        => type.HasCustomAttribute(typeof(TAttribute));
}