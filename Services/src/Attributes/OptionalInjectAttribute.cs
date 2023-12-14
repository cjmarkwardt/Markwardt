namespace Markwardt;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class OptionalInjectAttribute(Type? key = null) : BaseInjectAttribute
{
    public override bool IsOptional => true;

    public override object GetKey(Type type)
        => key ?? type;
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public class OptionalInjectAttribute<TKey> : OptionalInjectAttribute
{
    public OptionalInjectAttribute()
        : base(typeof(TKey)) { }
}