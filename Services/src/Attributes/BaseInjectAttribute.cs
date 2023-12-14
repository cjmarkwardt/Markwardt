namespace Markwardt;

public abstract class BaseInjectAttribute : Attribute
{
    public abstract bool IsOptional { get; }

    public abstract object GetKey(Type type);
}