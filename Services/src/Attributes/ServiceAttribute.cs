namespace Markwardt;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
public class ServiceAttribute(ServiceKind kind, Type? implementation = null) : BaseServiceAttribute
{
    public override IServiceDescription GetDescription(Type type)
        => Service.FromImplementation(kind, implementation ?? (type.IsInterface ? Type.GetType(type.Name[1..]) : type));
}