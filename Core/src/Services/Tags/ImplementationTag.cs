namespace Markwardt;

public class ImplementationTag<T> : IServiceTag
    where T : class
{
    public IServiceDescription? Default => Service.FromImplementation<T>(Kind);

    public virtual ServiceKind Kind => ServiceKind.Singleton;
}