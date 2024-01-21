namespace Markwardt;

public interface IServiceBuilder
{
    ValueTask<object> Build(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments = null);
}

public class ServiceBuilder(AsyncFunc<IServiceResolver, IReadOnlyDictionary<string, object?>?, object> factory) : IServiceBuilder
{
    public async ValueTask<object> Build(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments = null)
        => await factory(services, arguments);
}