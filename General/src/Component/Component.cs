namespace Markwardt;

public interface IComponent : IMultiDisposable, IDisposalViewer, IDisposalTracker, ILogger, INotifyPropertyChanged, INotifyPropertyChanging
{
    IComponent? Parent { get; set; }
}

public class Component : ManagedAsyncDisposable, IComponent
{
    public Component()
    {
        parentManager = new ComponentParentManager(this).DisposeWith(this);
        logReported.DisposeWith(this);
    }

    private readonly IComponentParentManager parentManager;

    private readonly Subject<LogReport> logReported = new();

    public IObservable<LogReport> LogReported => logReported;

    public IComponent? Parent { get => parentManager.Parent; set => parentManager.Parent = value; }

    public void Log(LogReport report)
        => logReported.OnNext(report);
}