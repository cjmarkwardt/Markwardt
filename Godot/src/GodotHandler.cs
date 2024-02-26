namespace Markwardt.Godot;

public class GodotHandler : IServiceHandler
{
    public IServiceDescription? Get(object key)
    {
        if (key is Type type && type.TryGetCustomAttribute(out NodeServiceAttribute? nodeAttribute))
        {
            return Service.FromDelegate(ServiceKind.Singleton, async services => (await services.RequireDefault<INodeTree>()).Descend(nodeAttribute.Path ?? $"Game/{type.Name[1..]}"));
        }

        return null;
    }
}