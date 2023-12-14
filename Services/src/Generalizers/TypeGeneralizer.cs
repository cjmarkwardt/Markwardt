namespace Markwardt;

public class TypeGeneralizer(Type type) : Generalizer(type.Name, GetTypeParameters(type).Select(x => x.Name.ToLower()).ToList())
{
    private static IEnumerable<Type> GetTypeParameters(Type type)
    {
        if (type.IsGenericTypeDefinition)
        {
            return type.GetGenericArguments();
        }
        else
        {
            return Enumerable.Empty<Type>();
        }
    }

    public Type Specify(IReadOnlyDictionary<string, Type> typeArguments)
    {
        if (type.IsGenericTypeDefinition)
        {
            return type.MakeGenericType(type.GetGenericArguments().Select(x => typeArguments[x.Name]).ToArray());
        }
        else
        {
            return type;
        }
    }

    public Type Close(IReadOnlyDictionary<string, object?> arguments)
        => Specify(GetTypeArguments(arguments));
}