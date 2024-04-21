namespace Markwardt;

public class MethodGeneralizer(MethodBase method) : Generalizer(method.Name, GetTypeParameters(method).Select(x => x.Name.ToLower()).ToList())
{
    private static IEnumerable<Type> GetTypeParameters(MethodBase method)
    {
        if (method is MethodInfo directMethod && directMethod.IsGenericMethodDefinition)
        {
            return directMethod.GetGenericArguments();
        }
        else
        {
            return Enumerable.Empty<Type>();
        }
    }

    public MethodBase Specify(IReadOnlyDictionary<string, Type> typeArguments)
    {
        if (method is MethodInfo directMethod && directMethod.IsGenericMethodDefinition)
        {
            return directMethod.MakeGenericMethod(directMethod.GetGenericArguments().Select(x => typeArguments[x.Name]).ToArray());
        }
        else
        {
            return method;
        }
    }

    public MethodBase Close(IReadOnlyDictionary<string, object?> arguments)
        => Specify(GetTypeArguments(arguments));
}