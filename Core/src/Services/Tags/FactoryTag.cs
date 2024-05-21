namespace Markwardt;

public abstract class FactoryTag<T> : DelegateTag
{
    protected override sealed ValueTask<object> Create(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments)
        => ValueTask.FromResult<object>((Factory<T>)Build);

    protected abstract ValueTask<T> Build();
}