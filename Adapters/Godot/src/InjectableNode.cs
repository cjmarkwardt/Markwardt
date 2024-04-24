namespace Markwardt;

public abstract partial class InjectableNode : Node, IInjectable, IExtendedDisposable, INode
{
    protected InjectableNode()
        => injector = new(this);

    private readonly Injector<InjectableNode> injector;

    public IScene? Parent => this.Generalize().Parent;

    public IEnumerable<INode> Children => this.Generalize().Children;

    public void Add(INode child)
        => this.Generalize().Add(child);

    public void Remove(INode child)
        => this.Generalize().Remove(child);

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