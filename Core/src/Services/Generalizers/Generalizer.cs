namespace Markwardt;

public class Generalizer(string targetName, IEnumerable<string> typeParameters)
{
    public IReadOnlyDictionary<string, Type> GetTypeArguments(IReadOnlyDictionary<string, object?>? arguments)
    {
        if (!typeParameters.Any())
        {
            return ImmutableDictionary<string, Type>.Empty;
        }

        Dictionary<string, Type> typeArguments = [];
        foreach (string typeParameter in typeParameters)
        {
            if (arguments != null && arguments.TryGetValue(typeParameter.ToLower(), out object? argument))
            {
                if (argument is Type type)
                {
                    typeArguments.Add(typeParameter.ToLower(), type);
                }
                else
                {
                    throw new InvalidOperationException($"Argument given for type parameter {typeParameter} was not a type in generic target {targetName}");
                }
            }
            else
            {
                throw new InvalidOperationException($"No type argument given for type parameter {typeParameter} in generic target {targetName}");
            }
        }

        return typeArguments;
    }
}