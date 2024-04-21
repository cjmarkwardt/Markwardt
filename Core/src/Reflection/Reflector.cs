namespace Markwardt;

public static class Reflector
{
    public static MethodInfo Reflect(Delegate target, params Type[] typeArguments)
        => typeArguments.Length == 0 ? target.Method : target.Method.GetGenericMethodDefinition().MakeGenericMethod(typeArguments);
}