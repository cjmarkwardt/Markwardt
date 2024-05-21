namespace Markwardt;

public abstract partial class GodotInitiator : Node
{
    public override async void _Ready()
    {
        base._Ready();
        
        IServiceContainer services = CreateContainer();
        services.Configure<IScene>(Service.FromImplementation<GodotScene>(ServiceKind.Singleton));
        services.Configure<INodeFinder>(Service.FromImplementation<GodotNodeFinder>(ServiceKind.Singleton));
        (await services.RequireTag<GlobalLoggersTag, IList<object>>()).Add(await services.Create<GodotLogger>());
        Configure(services);
        GlobalServices.Initialize(services);
        await this.RouteLogsToTop(services);
        await Start(services);
    }

    protected virtual IServiceContainer CreateContainer()
        => new ServiceContainer(new DefaultHandler().Concat(new GodotHandler()));

    protected virtual void Configure(IServiceConfiguration services) { }

    protected abstract ValueTask Start(IServiceResolver services);
}