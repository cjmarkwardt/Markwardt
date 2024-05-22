namespace Markwardt;

[AttributeUsage(AttributeTargets.Interface)]
public class SegmentAttribute(string? name = null) : Attribute
{
    public string? Name => name;
}