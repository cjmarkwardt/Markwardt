namespace Markwardt;

public abstract class SourceTag<TSource, T> : IServiceTag
    where T : notnull
{
    public IServiceDescription? Default => Service.FromSourceDefault<TSource, T>(Kind, Create);

    public virtual ServiceKind Kind => ServiceKind.Singleton;

    protected abstract ValueTask<T> Create(TSource source);
}

public abstract class SourceTag<TSourceTag, TSource, T> : IServiceTag
    where T : notnull
{
    public IServiceDescription? Default => Service.FromSourceTag<TSourceTag, TSource, T>(Kind, Create);

    public virtual ServiceKind Kind => ServiceKind.Singleton;

    protected abstract ValueTask<T> Create(TSource source);
}