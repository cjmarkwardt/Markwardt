namespace Markwardt;

public static class ObservableExtensions
{
    public static IDisposable Subscribe<T>(this IObservable<T> observable, Action onNext)
        => observable.Subscribe(_ => onNext());
}