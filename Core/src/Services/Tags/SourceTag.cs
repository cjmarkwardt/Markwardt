namespace Markwardt;

public abstract class SourceTag<TSource> : IServiceTag
{
    public IServiceDescription? Default => Service.FromSourceDefault<TSource>(Kind, Create);

    public virtual ServiceKind Kind => ServiceKind.Singleton;

    protected abstract ValueTask<object> Create(TSource source);
}

public abstract class SourceTag<TSourceTag, TSource> : IServiceTag
{
    public IServiceDescription? Default => Service.FromSourceTag<TSourceTag, TSource>(Kind, Create);

    public virtual ServiceKind Kind => ServiceKind.Singleton;

    protected abstract ValueTask<object> Create(TSource source);
}