namespace Markwardt.Godot;

public abstract partial class GodotInitiator : Node3D
{
    public override async void _Ready()
    {
        base._Ready();
        
        IServiceContainer services = CreateContainer();
        services.Configure(typeof(INodeTree), Service.FromInstance(true, new NodeTree(GetTree().Root)));
        (await services.RequireTag<GlobalLoggersTag, IList<object>>()).Add(await services.Create<GodotLogger>());
        Configure(services);
        GlobalServices.Initialize(services);
        await Start(services);
    }

    protected virtual IServiceContainer CreateContainer()
        => new ServiceContainer(new DefaultHandler().Concat(new GodotHandler()));

    protected virtual void Configure(IServiceConfiguration services) { }

    protected abstract ValueTask Start(IServiceResolver services);
}