namespace Markwardt;

public class ServiceRouter(object key, IServiceResolver? resolver = null) : IServiceBuilder
{
    private readonly IServiceResolver? resolver = resolver;

    public async ValueTask<object> Build(IServiceResolver resolver, IReadOnlyDictionary<string, object?>? arguments = null)
        => await (this.resolver ?? resolver).Require(key);
}
