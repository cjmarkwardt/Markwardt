namespace Markwardt;

public class ImplementationTag<T> : IServiceTag
{
    public IServiceDescription? Default => Service.FromImplementation<T>(Kind);

    public virtual ServiceKind Kind => ServiceKind.Singleton;
}