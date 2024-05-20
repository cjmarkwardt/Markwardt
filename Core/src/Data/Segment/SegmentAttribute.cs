namespace Markwardt;

[AttributeUsage(AttributeTargets.Interface)]
public class SegmentAttribute(string name) : Attribute
{
    public string Name => name;
}