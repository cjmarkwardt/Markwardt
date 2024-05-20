namespace Markwardt;

[AttributeUsage(AttributeTargets.Interface)]
public class EntitySectionAttribute(string name) : Attribute
{
    public string Name => name;
}