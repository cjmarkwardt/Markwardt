namespace Markwardt;

public interface IDisposable<out T> : IMultiDisposable
{
    bool IsDisposed { get; }
    T Value { get; }

    IDisposable<TCasted> Cast<TCasted>()
        => this.Cast<T, TCasted>();
}

public static class DisposableWrapperExtensions
{
    public static IDisposable<TSelected> Select<T, TSelected>(this IDisposable<T> disposable, Func<T, TSelected> selector)
        => new Disposable<TSelected>(selector(disposable.Value), [disposable]);

    public static IDisposable<TCasted> Cast<T, TCasted>(this IDisposable<T> disposable)
        => disposable.Select(x => (TCasted)(object?)x!);
}

public sealed class Disposable<T>(T value, IEnumerable<object> disposables) : IDisposable<T>
{
    public bool IsDisposed { get; private set; }

    private readonly T value = value;
    public T Value => !IsDisposed ? value : throw new ObjectDisposedException(GetType().Name);

    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            value.TryDispose();
            disposables.ForEach(x => x.TryDispose());
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
            await value.TryDisposeAsync();
            await Task.WhenAll(disposables.Select(x => x.TryDisposeAsync().AsTask()));
        }
    }
}