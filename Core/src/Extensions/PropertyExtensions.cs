namespace Markwardt;

public static class PropertyExtensions
{
    public static bool IsInitOnly(this PropertyInfo property)
        => property.SetMethod?.ReturnParameter.GetRequiredCustomModifiers().Contains(typeof(IsExternalInit)) == true;
}