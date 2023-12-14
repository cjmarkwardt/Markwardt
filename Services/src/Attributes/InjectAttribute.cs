namespace Markwardt;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class InjectAttribute(Type? key = null) : BaseInjectAttribute
{
    public override bool IsOptional => false;

    public override object GetKey(Type type)
        => key ?? type;
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class InjectAttribute<TKey> : InjectAttribute
{
    public InjectAttribute()
        : base(typeof(TKey)) { }
}