namespace Markwardt;

public class DefaultHandler : IServiceHandler
{
    public IServiceDescription? Get(object key)
    {
        if (key is Type type && type.TryGetCustomAttribute(out BaseServiceAttribute? serviceAttribute))
        {
            return serviceAttribute.GetDescription(type);
        }

        return null;
    }
}