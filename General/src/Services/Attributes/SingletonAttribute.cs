namespace Markwardt;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
public class SingletonAttribute(Type? implementation = null) : ServiceAttribute(ServiceKind.Singleton, implementation);

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
public class SingletonAttribute<TImplementation> : SingletonAttribute
    where TImplementation : class
{
    public SingletonAttribute()
        : base(typeof(TImplementation)) { }
}