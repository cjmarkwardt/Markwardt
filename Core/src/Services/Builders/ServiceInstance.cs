namespace Markwardt;

public class ServiceInstance(object instance) : IServiceBuilder
{
    public ValueTask<object> Build(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments = null)
        => new(instance);
}