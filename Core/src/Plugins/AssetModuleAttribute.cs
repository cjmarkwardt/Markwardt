namespace Markwardt;

[AttributeUsage(AttributeTargets.Assembly)]
public class AssetModuleAttribute(string id, Type? profile = null) : Attribute
{
    public string Id => id;
    public Type? Profile => profile;
}