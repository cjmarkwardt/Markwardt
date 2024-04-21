namespace Markwardt;

public class DefaultHandler : IServiceHandler
{
    public IServiceDescription? Get(object key)
    {
        if (key is Type type && type.TryGetCustomAttribute(out BaseServiceAttribute? attribute))
        {
            return attribute.GetDescription(type);
        }

        return null;
    }
}