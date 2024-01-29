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

    public void Log(string category, string message, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => Log(LogReport.FromCaller(category, message, sourcePath, sourceLine));

    public void Error(string message, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => Log("Error", message, sourcePath, sourceLine);

    public void Error(Exception exception, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
        => Error(exception.GetRecursiveMessage(), sourcePath, sourceLine);

    public void Error(Failable failable, [CallerFilePath] string sourcePath = "", [CallerLineNumber] int sourceLine = 0)
    {
        if (failable.Exception != null)
        {
            Error(failable.Exception, sourcePath, sourceLine);
        }
    }
}