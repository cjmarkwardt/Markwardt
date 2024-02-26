namespace Markwardt.Godot;

public abstract partial class GodotStarter : Node3D
{
    protected abstract string StartScene { get; }

    public override void _Ready()
    {
        base._Ready();
        
        IServiceContainer services = CreateContainer();
        services.Configure(new GodotPackage());
        services.Configure(typeof(INodeTree), Service.FromInstance(true, new NodeTree(GetTree().Root)));
        Configure(services);
        GlobalServices.SetResolver(services);
        GetTree().SetScene(StartScene);
    }

    protected virtual IServiceContainer CreateContainer()
        => new ServiceContainer(new CompositeHandler([new DefaultHandler(), new GodotHandler()]));

    protected virtual void Configure(IServiceConfiguration services) { }
}