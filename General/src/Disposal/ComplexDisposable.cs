namespace Markwardt;

public interface IComplexDisposable : IChainableDisposable, ITrackableDisposable, INotifyPropertyChanged
{
    IComplexDisposable? DisposalParent { get; set; }
}

public static class ComplexDisposableExtensions
{
    public static IDisposable RunInBackground(this IComplexDisposable disposable, AsyncAction<IDisposable> action, IEnumerable<string>? logCategory = null, object? logSource = null, [CallerFilePath] string? logLocationPath = null, [CallerLineNumber] int logLocationLine = -1)
    {
        disposable.Verify();

        return disposable.Fork(async (cancel, cancellation) =>
        {
            CancellationTokenSource linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation, disposable.DisposalToken);
            try
            {
                return await action(cancel, linkedCancellation.Token);
            }
            finally
            {
                linkedCancellation.Dispose();
            }
        }, logCategory, logSource, logLocationPath, logLocationLine);
    }

    public static void DisposeInBackground(this IComplexDisposable disposable, object? target)
        => disposable.RunInBackground(async (_, _) =>
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

    public static IDisposable LoopInBackground(this IComplexDisposable disposable, TimeSpan? interval, AsyncAction<IDisposable> action)
        => disposable.RunInBackground(async (cancel, cancellation) =>
        {
            while (!cancellation.IsCancellationRequested)
            {
                Failable tryAction = await action(cancel, cancellation);
                if (tryAction.IsFailure())
                {
                    return tryAction;
                }

                if (interval is not null)
                {
                    await TaskExtensions.TryDelay(interval.Value, cancellation);
                }
            }

            return Failable.Success();
        });

    public static IDisposable LoopInBackground(this IComplexDisposable disposable, AsyncAction<IDisposable> action)
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

    private readonly Subject<LogMessage> logged = new();
    public IObservable<LogMessage> Logged => logged;

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

    public virtual void Log(LogMessage message)
        => logged.OnNext(message);

    public IDisposable ChainDisposables(params object?[] disposables)
    {
        this.Verify();
        
        foreach (object disposable in disposables.WhereNotNull().Where(x => x is IDisposable || x is IAsyncDisposable))
        {
            chainedDisposables.Add(disposable);
        }

        return Disposable.Create(() => UnchainDisposables(disposables));
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

            logged.Dispose();
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

            logged.Dispose();
        }
    }

    protected virtual void PrepareDisposal() { }
    protected virtual void OnSharedDisposal() { }
    protected virtual void OnDisposal() { }
    protected virtual ValueTask OnAsyncDisposal() => ValueTask.CompletedTask;

    private void DetachParent()
    {
        disposalParent?.UnchainDisposables(this);
        disposalParent = null;

        logRouting?.Dispose();
        logRouting = null;
    }
}