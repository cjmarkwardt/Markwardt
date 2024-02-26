namespace Markwardt.Godot;

public class GodotPackage : IServicePackage
{
    public void Configure(IServiceConfiguration services)
    {
        services.Configure<ILogger>(Service.FromImplementation<GodotLogger>(ServiceKind.Singleton));
    }
}