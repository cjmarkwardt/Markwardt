namespace Markwardt;

public interface IRefreshable
{
    IObservable Refreshed { get; }
}

public interface IComplexDisposable : IChainableDisposable, ITrackableDisposable, ILogger, INotifyPropertyChanged, IRefreshable
{
    IComplexDisposable? DisposalParent { get; set; }
}

[SuppressMessage("Sonar Code Quality", "S3881")]
public class ComplexDisposable : ReactiveObject, IComplexDisposable
{
    public ComplexDisposable()
    {
        PropertyChanged += (_, _) => PushRefresh();
    }

    private readonly CancellationTokenSource disposalTokenSource = new();
    private readonly HashSet<object> chainedDisposables = [];

    private IDisposable? logRouting;

    private bool isDisposed;
    public bool IsDisposed
    {
        get => isDisposed;
        private set => this.RaiseAndSetIfChanged(ref isDisposed, value);
    }

    public CancellationToken DisposalToken => disposalTokenSource.Token;

    public virtual string LoggerName => GetType().Name;
    public virtual string DisposalName => LoggerName;

    private readonly Subject refreshed = new();
    public IObservable Refreshed => refreshed;

    private readonly Subject<LogMessage> logReported = new();
    public IObservable<LogMessage> LogReported => logReported;

    private IComplexDisposable? disposalParent;
    public IComplexDisposable? DisposalParent
    {
        get => disposalParent;
        set
        {
            this.Verify();

            DetachParent();

            if (value is not null)
            {
                disposalParent = value;
                this.DisposeWith(disposalParent);
                logRouting = this.RouteLogsTo(disposalParent);
            }
        }
    }

    public virtual void Log(LogMessage report)
        => PushLogReported(report);

    public void ChainDisposables(params object?[] disposables)
    {
        this.Verify();
        
        foreach (object disposable in disposables.WhereNotNull().Where(x => x is IDisposable || x is IAsyncDisposable))
        {
            chainedDisposables.Add(disposable);
        }
    }

    public void UnchainDisposables(params object?[] disposables)
    {
        foreach (object disposable in disposables.WhereNotNull())
        {
            chainedDisposables.Remove(disposable);
        }
    }

    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            PrepareDisposal();

            disposalTokenSource.Cancel();
            disposalTokenSource.Dispose();

            chainedDisposables.OfType<IDisposable>().ForEach(x => x.Dispose());
            chainedDisposables.Clear();

            DetachParent();

            OnSharedDisposal();
            OnDisposal();

            refreshed.Dispose();
            logReported.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            PrepareDisposal();

            await disposalTokenSource.CancelAsync();
            disposalTokenSource.Dispose();

            await chainedDisposables.OfType<IAsyncDisposable>().ForEachParallel(async x => await x.DisposeAsync());
            chainedDisposables.OfType<IDisposable>().ForEach(x => x.Dispose());
            chainedDisposables.Clear();

            DetachParent();

            OnSharedDisposal();
            await OnAsyncDisposal();

            refreshed.Dispose();
            logReported.Dispose();
        }
    }

    protected virtual void PrepareDisposal() { }
    protected virtual void OnSharedDisposal() { }
    protected virtual void OnDisposal() { }
    protected virtual ValueTask OnAsyncDisposal() => ValueTask.CompletedTask;

    protected void PushRefresh()
        => refreshed.OnNext();

    protected void PushLogReported(LogMessage report)
        => logReported.OnNext(report);

    private void DetachParent()
    {
        disposalParent?.UnchainDisposables(this);
        disposalParent = null;

        logRouting?.Dispose();
        logRouting = null;
    }
}