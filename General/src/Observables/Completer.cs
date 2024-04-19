namespace Markwardt;

public interface IGeneralCompleter
{
    bool IsCompleted { get; }
    bool IsSuccess { get; }
    bool IsFailure { get; }
    bool IsCancellation { get; }

    bool TryFail(Exception exception);
    bool TryCancel();
}

public interface ICompleter<T> : IGeneralCompleter
{
    Failable<T>? Result { get; }
    IObservable<Failable<T>> Completed { get; }

    bool TryComplete(Failable<T> result);
}

public interface ICompleter : IGeneralCompleter
{
    Failable? Result { get; }
    IObservable<Failable> Completed { get; }

    bool TryComplete(Failable result);
}

public static class CompleterExtensions
{
    public static AsyncSubject<Failable<T>> GetAwaiter<T>(this ICompleter<T> completer)
        => completer.Completed.GetAwaiter();

    public static AsyncSubject<Failable> GetAwaiter(this ICompleter completer)
        => completer.Completed.GetAwaiter();

    public static bool TrySucceed<T>(this ICompleter<T> completer, T result)
        => completer.TryComplete(result.AsFailable());

    public static bool TrySucceed(this ICompleter completer)
        => completer.TryComplete(Failable.Success());

    public static bool TryTimeout(this IGeneralCompleter completer)
        => completer.TryFail(new TimeoutException());

    public static void Complete<T>(this ICompleter<T> completer, Failable<T> result)
    {
        if (!completer.TryComplete(result))
        {
            throw new InvalidOperationException("Completer has already completed");
        }
    }

    public static void Complete(this ICompleter completer, Failable result)
    {
        if (!completer.TryComplete(result))
        {
            throw new InvalidOperationException("Completer has already completed");
        }
    }

    public static void Succeed<T>(this ICompleter<T> completer, T result)
    {
        if (!completer.TrySucceed(result))
        {
            throw new InvalidOperationException("Completer has already completed");
        }
    }

    public static void Succeed(this ICompleter completer)
    {
        if (!completer.TrySucceed())
        {
            throw new InvalidOperationException("Completer has already completed");
        }
    }

    public static void Fail(this IGeneralCompleter completer, Exception exception)
    {
        if (!completer.TryFail(exception))
        {
            throw new InvalidOperationException("Completer has already completed");
        }
    }

    public static void Cancel(this IGeneralCompleter completer)
    {
        if (!completer.TryCancel())
        {
            throw new InvalidOperationException("Completer has already completed");
        }
    }

    public static IDisposable AddTimeout(this IGeneralCompleter completer, TimeSpan timeout, TimeSpan? interval = null)
    {
        CancellationTokenSource cancellation = new();
        completer.AddTimeout(timeout, interval, cancellation.Token);
        return cancellation;
    }

    private static async void AddTimeout(this IGeneralCompleter completer, TimeSpan timeout, TimeSpan? interval, CancellationToken cancellation)
    {
        DateTime expiration = DateTime.Now + timeout;
        while (!completer.IsCompleted && DateTime.Now < expiration && !cancellation.IsCancellationRequested)
        {
            await TaskExtensions.TryDelay(interval ?? TimeSpan.FromMilliseconds(50), cancellation);
        }

        if (!cancellation.IsCancellationRequested)
        {
            completer.TryTimeout();
        }
    }

    public static IDisposable AddCancellation(this IGeneralCompleter completer, CancellationToken cancellation)
    {
        IDisposable? subscription = null;
        subscription = cancellation.Register(() =>
        {
            completer.TryCancel();
            subscription?.Dispose();
        });

        return subscription;
    }
}

public class Completer<T> : ICompleter<T>
{
    public Failable<T>? Result { get; private set; }

    private readonly ReplaySubject<Failable<T>> completed = new(1);
    public IObservable<Failable<T>> Completed => completed;

    public bool IsCompleted => Result is not null;
    public bool IsSuccess => Result is not null && Result.IsSuccess();
    public bool IsFailure => Result is not null && Result.IsFailure();
    public bool IsCancellation => Result is not null && Result.IsCancellation();

    public bool TryComplete(Failable<T> result)
    {
        if (Result is not null)
        {
            return false;
        }

        Result = result;
        completed.OnNext(result);
        completed.OnCompleted();
        return true;
    }

    public bool TryFail(Exception exception)
        => TryComplete(Failable.Fail<T>(exception));

    public bool TryCancel()
        => TryComplete(Failable.Cancel<T>());
}

public class Completer : ICompleter
{
    public Failable? Result { get; private set; }

    private readonly ReplaySubject<Failable> completed = new(1);
    public IObservable<Failable> Completed => completed;

    public bool IsCompleted => Result is not null;
    public bool IsSuccess => Result is not null && Result.IsSuccess();
    public bool IsFailure => Result is not null && Result.IsFailure();
    public bool IsCancellation => Result is not null && Result.IsCancellation();

    public bool TryComplete(Failable result)
    {
        if (Result is not null)
        {
            return false;
        }

        Result = result;
        completed.OnNext(result);
        completed.OnCompleted();
        return true;
    }

    public bool TryFail(Exception exception)
        => TryComplete(Failable.Fail(exception));

    public bool TryCancel()
        => TryComplete(Failable.Cancel());
}