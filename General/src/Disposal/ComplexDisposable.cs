namespace Markwardt;

public interface IComplexDisposable : IChainableDisposable, ITrackableDisposable, IExceptionContext, ILogger, INotifyPropertyChanged
{
    IComplexDisposable? DisposalParent { get; set; }
}

public static class ComplexDisposableExtensions
{
    public static IDisposable RunInBackground(this IComplexDisposable disposable, AsyncAction action)
    {
        disposable.Verify();

        CancellationTokenSource cancellation = CancellationTokenSource.CreateLinkedTokenSource(disposable.DisposalToken);

        TaskExtensions.Fork(async () =>
        {
            try
            {
                Failable tryAction = await action(cancellation.Token);
                if (tryAction.Exception is not null)
                {
                    disposable.PushException(tryAction.Exception);
                }
            }
            catch (Exception exception)
            {
                disposable.PushException(exception);
            }

            cancellation.Dispose();
        });

        return Disposable.Create(cancellation.Cancel);
    }

    public static void DisposeInBackground(this IComplexDisposable disposable, object? target)
        => disposable.RunInBackground(async _ =>
        {
            if (target is IAsyncDisposable targetAsyncDisposable)
            {
                await targetAsyncDisposable.DisposeAsync();
            }
            else if (target is IDisposable targetDisposable)
            {
                targetDisposable.Dispose();
            }

            return Failable.Success();
        });

    public static IDisposable LoopInBackground(this IComplexDisposable disposable, TimeSpan? interval, AsyncAction<StopLoopAction> action)
        => disposable.RunInBackground(async cancellation =>
        {
            CancellationTokenSource loopCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            while (!loopCancellation.IsCancellationRequested)
            {
                Failable tryAction = await action(() => loopCancellation.Cancel(), loopCancellation.Token);
                if (tryAction.Exception is not null)
                {
                    return tryAction;
                }

                if (interval is not null)
                {
                    await TaskExtensions.TryDelay(interval.Value, loopCancellation.Token);
                }
            }

            loopCancellation.Dispose();
            return Failable.Success();
        });

    public static IDisposable LoopInBackground(this IComplexDisposable disposable, AsyncAction<StopLoopAction> action)
        => disposable.LoopInBackground(null, action);
}

[SuppressMessage("Sonar Code Quality", "S3881")]
public class ComplexDisposable : ReactiveObject, IComplexDisposable
{
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

    private readonly Subject<LogMessage> logReported = new();
    public IObservable<LogMessage> LogReported => logReported;

    private readonly Subject<Exception> exceptionPushed = new();
    public IObservable<Exception> ExceptionPushed => exceptionPushed;

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

    public void PushException(Exception exception)
    {
        exceptionPushed.OnNext(exception);
        this.LogError(exception);
    }

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

            exceptionPushed.Dispose();
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

            exceptionPushed.Dispose();
            logReported.Dispose();
        }
    }

    protected virtual void PrepareDisposal() { }
    protected virtual void OnSharedDisposal() { }
    protected virtual void OnDisposal() { }
    protected virtual ValueTask OnAsyncDisposal() => ValueTask.CompletedTask;

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