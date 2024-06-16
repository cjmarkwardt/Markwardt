namespace Markwardt;

public static class NullExtensions
{
    public static bool NullableEquals(this object? x, object? y)
        => (x is null && y is null) || (x is not null && x.Equals(y));

    public static T NotNull<T>(this T? obj, string? message = null)
        where T : class
        => obj ?? throw new InvalidOperationException(message ?? "Value is null");

    public static bool TryNotNull<T>(this T? target, [NotNullWhen(true)] out T? output)
        where T : class
    {
        output = target;
        return target != null;
    }

    public static T ValueNotNull<T>(this T? obj, string? message = null)
        where T : struct
        => obj ?? throw new InvalidOperationException(message ?? "Value is null");
        
    public static bool TryValueNotNull<T>(this T? target, [NotNullWhen(true)] out T? output)
        where T : struct
    {
        output = target;
        return target != null;
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> items)
        where T : class
        => items.Where(x => x != null).Select(x => x!);

    public static IAsyncEnumerable<T> WhereNotNull<T>(this IAsyncEnumerable<T?> items)
        where T : class
        => items.Where(x => x != null).Select(x => x!);

    public static IAsyncEnumerable<T> WhereValueNotNull<T>(this IAsyncEnumerable<T?> items)
        where T : struct
        => items.Where(x => x.HasValue).Select(x => x!.Value);

    public static bool IsNullable(this PropertyInfo property) =>
        IsNullableHelper(property.PropertyType, property.DeclaringType, property.CustomAttributes);

    public static bool IsNullable(this FieldInfo field) =>
        IsNullableHelper(field.FieldType, field.DeclaringType, field.CustomAttributes);

    public static bool IsNullable(this ParameterInfo parameter) =>
        IsNullableHelper(parameter.ParameterType, parameter.Member, parameter.CustomAttributes);

    private static bool IsNullableHelper(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
    {
        if (memberType.IsValueType)
            return Nullable.GetUnderlyingType(memberType) != null;

        var nullable = customAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
        if (nullable != null && nullable.ConstructorArguments.Count == 1)
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                {
                    return (byte)args[0].Value! == 2;
                }
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte)attributeArgument.Value! == 2;
            }
        }

        for (var type = declaringType; type != null; type = type.DeclaringType)
        {
            var context = type.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
            if (context != null &&
                context.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments[0].ArgumentType == typeof(byte))
            {
                return (byte)context.ConstructorArguments[0].Value! == 2;
            }
        }

        return false;
    }
}