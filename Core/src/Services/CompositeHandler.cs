namespace Markwardt;

public class CompositeHandler(IEnumerable<IServiceHandler> handlers) : IServiceHandler
{
    public IServiceDescription? Get(object key)
    {
        foreach (IServiceHandler handler in handlers)
        {
            IServiceDescription? description = handler.Get(key);
            if (description != null)
            {
                return description;
            }
        }

        return null;
    }
}