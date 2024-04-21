namespace Markwardt;

public class RouteTag<T> : IServiceTag
{
    public IServiceDescription? Default => Service.FromKey(typeof(T));
}