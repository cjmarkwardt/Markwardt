namespace Markwardt;

public interface IChainableDisposable : IMultiDisposable
{
    void ChainDisposables(params object?[] disposables);
    void UnchainDisposables(params object?[] disposables);
}

public static class ChainableDisposableExtensions
{
    public static T DisposeWith<T>(this T disposable, IChainableDisposable tracker)
    {
        tracker.ChainDisposables(disposable);
        return disposable;
    }
}