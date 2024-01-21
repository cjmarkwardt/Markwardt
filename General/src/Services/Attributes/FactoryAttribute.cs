namespace Markwardt;

[AttributeUsage(AttributeTargets.Delegate)]
public class FactoryAttribute(Type? implementation = null, string? name = null, string[]? parameterNames = null, Type[]? parameterTypes = null) : SingletonAttribute(implementation)
{
    public string? Name => name;
    public IEnumerable<string>? ParameterNames => parameterNames;
    public IEnumerable<Type>? ParameterTypes => parameterTypes;
}

[AttributeUsage(AttributeTargets.Delegate)]
public class FactoryAttribute<TImplementation>(string? name = null, string[]? parameterNames = null, Type[]? parameterTypes = null) : FactoryAttribute(typeof(TImplementation), name, parameterNames, parameterTypes)
    where TImplementation : class;