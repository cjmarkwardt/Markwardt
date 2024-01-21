namespace Markwardt;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class ExportAttribute(string? name) : Attribute
{
    public string? Name => name;
}