namespace Markwardt;

public class ServiceRouter(object key, IServiceResolver? resolver = null) : IServiceBuilder
{
    private readonly IServiceResolver? resolver = resolver;

    public async ValueTask<object> Build(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments = null)
        => await (this.resolver ?? services).Require(key);
}
