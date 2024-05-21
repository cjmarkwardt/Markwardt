namespace Markwardt;

public abstract partial class InjectableNode : Node, IInjectable, IExtendedDisposable, INode
{
    protected InjectableNode()
        => injector = new(this);

    private readonly Injector<InjectableNode> injector;

	public required ITopLogger Logger { get; init; }

    public object? Parent { get => this.Generalize().Parent; set => this.Generalize().Parent = value; }

    public IEnumerable<object> Children => this.Generalize().Children;

    public void Add(object child)
        => this.Generalize().Add(child);

    public void Remove(object child)
        => this.Generalize().Remove(child);

    public override sealed void _Ready()
    {
        this.RouteLogsTo(Logger);
        injector.Ready();
    }

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