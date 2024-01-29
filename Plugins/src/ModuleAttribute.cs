namespace Markwardt;

[AttributeUsage(AttributeTargets.Assembly)]
public class ModuleAttribute(string id, string name, string author) : Attribute
{
    public string Id => id;
    public string Name => name;
    public string Author => author;
}