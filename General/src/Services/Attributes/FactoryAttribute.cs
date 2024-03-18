namespace Markwardt;

[AttributeUsage(AttributeTargets.Delegate)]
public class FactoryAttribute(Type? implementation = null, string? name = null, string[]? parameterNames = null, Type[]? parameterTypes = null) : SingletonAttribute(implementation)
{
    public string? Name => name;
    public IEnumerable<string>? ParameterNames => parameterNames;
    public IEnumerable<Type>? ParameterTypes => parameterTypes;

    public override IServiceDescription GetDescription(Type type)
        => Service.FromBuilder(ServiceKind.Singleton, base.GetDescription(type).Builder.ThroughDelegate(type));
}

[AttributeUsage(AttributeTargets.Delegate)]
public class FactoryAttribute<TImplementation>(string? name = null, string[]? parameterNames = null, Type[]? parameterTypes = null) : FactoryAttribute(typeof(TImplementation), name, parameterNames, parameterTypes)
    where TImplementation : class;