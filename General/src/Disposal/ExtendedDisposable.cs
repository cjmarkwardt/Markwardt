namespace Markwardt;

public interface IExtendedDisposable : IMultiDisposable;

public static class ExtendedDisposableExtensions
{
    private static readonly ConditionalWeakTable<IExtendedDisposable, Disposer> disposers = [];

    public static void TriggerDisposal(IExtendedDisposable disposable)
    {
        Disposer disposer = GetDisposer(disposable, false);

        if (!disposer.IsDisposed)
        {
            disposer.IsDisposed = true;
            disposer.Preparations.InvokeAll();

            disposable.SetDisposalParent(null);

            disposer.TokenSource.Cancel();
            disposer.TokenSource.Dispose();

            disposer.Children.OfType<IDisposable>().ForEach(x => x.Dispose());

            disposer.SharedActions.InvokeAll();
            disposer.Actions.InvokeAll();
        }
    }

    public static async ValueTask TriggerAsyncDisposal(IExtendedDisposable disposable)
    {
        Disposer disposer = GetDisposer(disposable, false);

        if (!disposer.IsDisposed)
        {
            disposer.IsDisposed = true;
            disposer.Preparations.InvokeAll();

            disposable.SetDisposalParent(null);

            await disposer.TokenSource.CancelAsync();
            disposer.TokenSource.Dispose();

            await disposer.Children.OfType<IAsyncDisposable>().Select(x => x.DisposeAsync().AsTask()).InvokeAll();
            disposer.Children.OfType<IDisposable>().ForEach(x => x.Dispose());

            disposer.SharedActions.InvokeAll();
            await disposer.AsyncActions.InvokeAll();
        }
    }

    public static bool IsDisposed(this IExtendedDisposable disposable)
        => GetDisposer(disposable, false).IsDisposed;

    public static CancellationToken GetDisposalToken(this IExtendedDisposable disposable)
        => GetDisposer(disposable, false).TokenSource.Token;

    public static void VerifyUndisposed(this IExtendedDisposable disposable)
        => ObjectDisposedException.ThrowIf(disposable.IsDisposed(), disposable);

    public static IExtendedDisposable? GetDisposalParent(this IExtendedDisposable disposable)
        => GetDisposer(disposable, false).Parent;

    public static void SetDisposalParent(this IExtendedDisposable disposable, IExtendedDisposable? parent)
    {
        Disposer disposer = GetDisposer(disposable, true);

        if (disposer.Parent is not null)
        {
            GetDisposer(disposer.Parent, false).Children.Remove(disposable);
        }

        disposer.Parent = null;

        if (parent is not null)
        {
            disposer.Parent = parent;
            GetDisposer(parent, true).Children.Add(disposable);
        }
    }

    public static T DisposeWith<T>(this T disposable, IExtendedDisposable parent)
    {
        if (disposable is IExtendedDisposable extended)
        {
            extended.SetDisposalParent(parent);
        }
        else
        {
            GetDisposer(parent, true).Children.Add(disposable);
        }
        
        return disposable;
    }

    public static T DisposeWithout<T>(this T disposable, IExtendedDisposable parent)
    {
        if (disposable is IExtendedDisposable extended)
        {
            if (extended.GetDisposalParent() == parent)
            {
                extended.SetDisposalParent(null);
            }
        }
        else
        {
            GetDisposer(parent, false).Children.Remove(disposable);
        }
        
        return disposable;
    }

    public static IDisposable AddDisposalPeparation(this IExtendedDisposable disposable, Action action)
    {
        Disposer disposer = GetDisposer(disposable, true);
        disposer.Preparations.Add(action);
        return Disposable.Create(() => disposer.Preparations.Remove(action));
    }

    public static IDisposable AddSharedDisposal(this IExtendedDisposable disposable, Action action)
    {
        Disposer disposer = GetDisposer(disposable, true);
        disposer.SharedActions.Add(action);
        return Disposable.Create(() => disposer.SharedActions.Remove(action));
    }

    public static IDisposable AddDisposal(this IExtendedDisposable disposable, Action action)
    {
        Disposer disposer = GetDisposer(disposable, true);
        disposer.Actions.Add(action);
        return Disposable.Create(() => disposer.Actions.Remove(action));
    }

    public static IDisposable AddAsyncDisposal(this IExtendedDisposable disposable, Func<ValueTask> action)
    {
        Disposer disposer = GetDisposer(disposable, true);
        disposer.AsyncActions.Add(action);
        return Disposable.Create(() => disposer.AsyncActions.Remove(action));
    }

    public static IDisposable AddFullDisposal(this IExtendedDisposable disposable, Action onPrepareDisposal, Action onSharedDisposal, Action onDisposal, Func<ValueTask> onAsyncDisposal)
    {
        IDisposable prepareSubscription = disposable.AddDisposalPeparation(onPrepareDisposal);
        IDisposable sharedDisposalSubscription = disposable.AddSharedDisposal(onSharedDisposal);
        IDisposable disposalSubscription = disposable.AddDisposal(onDisposal);
        IDisposable asyncDisposalSubscription = disposable.AddAsyncDisposal(onAsyncDisposal);
        return Disposable.Create(() =>
        {
            prepareSubscription.Dispose();
            sharedDisposalSubscription.Dispose();
            disposalSubscription.Dispose();
            asyncDisposalSubscription.Dispose();
        });
    }
    
    public static IDisposable RunInBackground(this IExtendedDisposable disposable, AsyncAction<IDisposable> action, IEnumerable<string>? logCategory = null, object? logSource = null, [CallerFilePath] string? logLocationPath = null, [CallerLineNumber] int logLocationLine = -1)
    {
        disposable.VerifyUndisposed();

        return disposable.Fork(async (cancel, cancellation) =>
        {
            CancellationTokenSource linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation, disposable.GetDisposalToken());
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

    public static void DisposeInBackground(this IExtendedDisposable disposable, object? target)
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

    public static IDisposable LoopInBackground(this IExtendedDisposable disposable, TimeSpan? interval, AsyncAction<IDisposable> action)
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

    public static IDisposable LoopInBackground(this IExtendedDisposable disposable, AsyncAction<IDisposable> action)
        => disposable.LoopInBackground(null, action);

    private static Disposer GetDisposer(IExtendedDisposable disposable, bool verify)
    {
        if (verify)
        {
            disposable.VerifyUndisposed();
        }

        return disposers.GetOrCreateValue(disposable);
    }

    private sealed class Disposer
    {
        public CancellationTokenSource TokenSource { get; } = new();
        public HashSet<object?> Children { get; } = [];
        public List<Action> Preparations { get; } = [];
        public List<Action> SharedActions { get; } = [];
        public List<Action> Actions { get; } = [];
        public List<Func<ValueTask>> AsyncActions { get; } = [];

        public bool IsDisposed { get; set; }
        public IExtendedDisposable? Parent { get; set; }
    }
}

public class ExtendedDisposable : ReactiveObject, IExtendedDisposable
{
    public ExtendedDisposable()
        => this.AddFullDisposal(OnPrepareDisposal, OnSharedDisposal, OnDisposal, OnAsyncDisposal);

    public void Dispose()
        => ExtendedDisposableExtensions.TriggerDisposal(this);

    public async ValueTask DisposeAsync()
        => await ExtendedDisposableExtensions.TriggerAsyncDisposal(this);

    protected virtual void OnPrepareDisposal() { }
    protected virtual void OnSharedDisposal() { }
    protected virtual void OnDisposal() { }
    protected virtual ValueTask OnAsyncDisposal() => ValueTask.CompletedTask;
}