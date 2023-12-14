namespace Markwardt;

[AttributeUsage(AttributeTargets.Property)]
public class DataDefaultAttribute(object? value) : Attribute
{
    public object? Value => value;
}