namespace Markwardt;

public static class ObservableExtensions
{
    public static IObservable Generalize<T>(this IObservable<T> observable)
        => new GeneralObservable(observable.Select(_ => true));

    public static IDisposable Subscribe<T>(this IObservable<T> observable, Action onNext)
        => observable.Subscribe(_ => onNext());

    public static IDisposable WeakSubscribe<T, TSubscriber>(this IObservable<T> observable, TSubscriber subscriber, Action<TSubscriber, T> action)
        where TSubscriber : class
    {
        if (action.Target is not null)
        {
            throw new InvalidOperationException($"Weak subscription is holding a strong reference to {action.Target}");
        }

        IDisposable? subscription = null;
        WeakReference<TSubscriber> reference = new(subscriber);

        void OnNext(T item)
        {
            if (reference.TryGetTarget(out TSubscriber? subscriber))
            {
                action(subscriber, item);
            }
            else
            {
                subscription?.Dispose();
            }
        }

        void OnError(Exception exception)
            => subscription?.Dispose();

        void OnComplete()
            => subscription?.Dispose();

        subscription = observable.Subscribe(OnNext, OnError, OnComplete);
        return subscription;
    }

    public static IDisposable WeakSubscribe<T, TSubscriber>(this IObservable<T> observable, TSubscriber subscriber, Action<TSubscriber> action)
        where TSubscriber : class
        => observable.WeakSubscribe(subscriber, (subscriber, _) => action(subscriber));
}