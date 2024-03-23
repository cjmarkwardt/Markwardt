namespace Markwardt.Godot;

public abstract partial class InjectableControl : Control
{
    private bool isInitialized;

    public override sealed async void _Ready()
    {
        await GlobalServices.Inject(this);
        await OnReady();
        isInitialized = true;
    }

    public override sealed void _Process(double delta)
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