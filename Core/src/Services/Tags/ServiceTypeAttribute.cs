namespace Markwardt;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceTypeAttribute(params Type[] types) : Attribute
{
    public IReadOnlyList<Type> Types => types;
}