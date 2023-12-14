namespace Markwardt;

public abstract class BaseServiceAttribute : Attribute
{
    public abstract IServiceDescription GetDescription(Type type);
}