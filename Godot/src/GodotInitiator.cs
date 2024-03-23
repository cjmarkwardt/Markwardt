namespace Markwardt.Godot;

public abstract partial class GodotInitiator : Node3D
{
    public override async void _Ready()
    {
        base._Ready();
        
        IServiceContainer services = CreateContainer();
        services.Configure(new GodotPackage());
        services.Configure(typeof(INodeTree), Service.FromInstance(true, new NodeTree(GetTree().Root)));
        Configure(services);
        GlobalServices.Initialize(services);
        await Start(services);
    }

    protected virtual IServiceContainer CreateContainer()
        => new ServiceContainer(new DefaultHandler().Concat(new GodotHandler()));

    protected virtual void Configure(IServiceConfiguration services) { }

    protected abstract ValueTask Start(IServiceResolver services);
}