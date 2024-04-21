namespace Markwardt.Godot;

public abstract partial class InjectableControl : Control, IInjectable, IExtendedDisposable
{
    protected InjectableControl()
        => injector = new(this);

    private readonly Injector<InjectableControl> injector;

    public override sealed void _Ready()
        => injector.Ready();

    public override sealed void _Process(double delta)
        => injector.Process(delta);

    public async ValueTask DisposeAsync()
        => await ExtendedDisposableExtensions.TriggerAsyncDisposal(this);

    void IInjectable.SetName(string name)
        => Name = name;

    async ValueTask IInjectable.OnReady(CancellationToken cancellation)
        => await OnReady(cancellation);

    void IInjectable.OnProcess(double delta)
        => OnProcess(delta);

    protected virtual ValueTask OnReady(CancellationToken cancellation)
        => ValueTask.CompletedTask;

    protected virtual void OnProcess(double delta) { }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            ExtendedDisposableExtensions.TriggerDisposal(this);
        }
    }
}