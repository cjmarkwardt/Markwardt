namespace Markwardt;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum)]
public class ExportTypeAttribute(string type) : Attribute
{
    public string Type => type;
}