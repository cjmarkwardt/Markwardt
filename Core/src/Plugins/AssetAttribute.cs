namespace Markwardt;

[AttributeUsage(AttributeTargets.Class)]
public class AssetAttribute(string? id = null) : Attribute
{
    public static Func<string, ValueTask<object?>> CreateAssetFactory(IServiceResolver resolver, IEnumerable<Type> types)
    {
        Dictionary<string, Func<ValueTask<object>>> assets = [];
        foreach (Type type in types)
        {
            AssetAttribute? attribute = type.GetCustomAttribute<AssetAttribute>();
            if (attribute is not null)
            {
                assets.Add(attribute.Id ?? (type.Namespace is null ? type.Name : $"{type.Namespace}.{type.Name}"), async () => await resolver.Create(type));
            }
        }

        return async id => assets.TryGetValue(id, out Func<ValueTask<object>>? factory) ? await factory() : null;
    }

    public string? Id => id;
}