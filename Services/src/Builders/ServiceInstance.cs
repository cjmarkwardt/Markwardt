namespace Markwardt;

public class ServiceInstance(object instance) : IServiceBuilder
{
    public ValueTask<object> Build(IServiceResolver resolver, IReadOnlyDictionary<string, object?>? arguments = null)
        => new(instance);
}