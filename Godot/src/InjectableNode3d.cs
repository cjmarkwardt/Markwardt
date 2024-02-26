namespace Markwardt.Godot;

public abstract partial class InjectableNode3d : Node3D
{
    private bool isInitialized;

    public override async void _Ready()
    {
        await this.TryAutoInitializeServices();
        await OnReady();
        isInitialized = true;
    }

    public override void _Process(double delta)
    {
        if (isInitialized)
        {
            OnProcess(delta);
        }
    }

    protected virtual ValueTask OnReady()
        => ValueTask.CompletedTask;

    protected virtual void OnProcess(double delta) { }
}