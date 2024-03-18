namespace Markwardt;

public class StubTag : DelegateTag
{
    protected override ValueTask<object> Create(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments)
        => throw new InvalidOperationException("Service does not exist");
}