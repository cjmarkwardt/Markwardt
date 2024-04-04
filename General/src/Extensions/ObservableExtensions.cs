namespace Markwardt;

public static class ObservableExtensions
{
    public static IObservable Generalize<T>(this IObservable<T> observable)
        => new GeneralObservable(observable.Select(_ => true));

    public static IDisposable Subscribe<T>(this IObservable<T> observable, Action onNext)
        => observable.Subscribe(_ => onNext());
}