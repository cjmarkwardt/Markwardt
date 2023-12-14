namespace Markwardt;

public static class AttributeExtensions
{
    public static bool TryGetCustomAttribute(this MemberInfo member, Type attributeType, [NotNullWhen(true)] out Attribute? attribute)
    {
        attribute = member.GetCustomAttribute(attributeType);
        return attribute != null;
    }
    
    public static bool TryGetCustomAttribute<TAttribute>(this MemberInfo member, [NotNullWhen(true)] out TAttribute? attribute)
        where TAttribute : Attribute
    {
        attribute = member.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }

    public static bool HasCustomAttribute(this MemberInfo member, Type attributeType)
        => member.GetCustomAttribute(attributeType) != null;

    public static bool HasCustomAttribute<TAttribute>(this MemberInfo member)
        where TAttribute : Attribute
        => member.GetCustomAttribute<TAttribute>() != null;

    public static bool TryGetCustomAttribute(this ParameterInfo member, Type attributeType, [NotNullWhen(true)] out Attribute? attribute)
    {
        attribute = member.GetCustomAttribute(attributeType);
        return attribute != null;
    }
    
    public static bool TryGetCustomAttribute<TAttribute>(this ParameterInfo member, [NotNullWhen(true)] out TAttribute? attribute)
        where TAttribute : Attribute
    {
        attribute = member.GetCustomAttribute<TAttribute>();
        return attribute != null;
    }

    public static bool HasCustomAttribute(this ParameterInfo member, Type attributeType)
        => member.GetCustomAttribute(attributeType) != null;

    public static bool HasCustomAttribute<TAttribute>(this ParameterInfo member)
        where TAttribute : Attribute
        => member.GetCustomAttribute<TAttribute>() != null;
}