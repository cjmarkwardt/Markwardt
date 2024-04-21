namespace Markwardt;

[AttributeUsage(AttributeTargets.Property)]
public class DataRelationshipAttribute(string propertyName) : Attribute
{
    public string PropertyName => propertyName;
}