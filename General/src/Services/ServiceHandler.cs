namespace Markwardt;

public interface IServiceHandler
{
    IServiceDescription? Get(object key);
}

public static class ServiceHandlerExtensions
{
    public static IServiceHandler Concat(this IServiceHandler handler, IServiceHandler fallback)
        => new CompositeHandler([handler, fallback]);
}