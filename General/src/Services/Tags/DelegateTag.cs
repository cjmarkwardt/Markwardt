namespace Markwardt;

public abstract class DelegateTag : IServiceTag
{
    public IServiceDescription? Default => Service.FromDelegate(Kind, Create);

    public virtual ServiceKind Kind => ServiceKind.Singleton;

    protected abstract ValueTask<object> Create(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments);
}