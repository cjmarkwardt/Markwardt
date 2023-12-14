namespace Markwardt;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
public class TransientAttribute(Type? implementation = null) : ServiceAttribute(ServiceKind.Transient, implementation);

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
public class TransientAttribute<TImplementation> : TransientAttribute
    where TImplementation : class
{
    public TransientAttribute()
        : base(typeof(TImplementation)) { }
}