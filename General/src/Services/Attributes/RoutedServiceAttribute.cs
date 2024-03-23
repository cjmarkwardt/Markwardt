namespace Markwardt;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
public class RoutedServiceAttribute(object Key) : BaseServiceAttribute
{
    public override IServiceDescription GetDescription(Type type)
        => Service.FromKey(Key);
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Enum)]
public class RoutedServiceAttribute<TKey>() : RoutedServiceAttribute(typeof(TKey));