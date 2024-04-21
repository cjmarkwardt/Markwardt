namespace Markwardt;

public class McMasterPluginsPackage : IServicePackage
{
    public void Configure(IServiceConfiguration services)
    {
        services.Configure<IDynamicAssembly.Factory>(Service.FromFactory<McMasterDynamicAssembly, IDynamicAssembly.Factory>());
    }
}