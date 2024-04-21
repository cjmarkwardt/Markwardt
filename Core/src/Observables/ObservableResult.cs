namespace Markwardt;

public interface IObservableResult<out T> : IObservable<T>
{
    T Result { get; }
    bool HasResult { get; }
}

public interface ISourceResult<T> : IObservableResult<T>
{
    void SetResult(T result);
}

public record SourceResult<T> : ISourceResult<T>
{
    private readonly ReplaySubject<T> subject = new(1);

    private T result = default!;

    public T Result => HasResult ? result : throw new InvalidOperationException("Result is not set");

    public bool HasResult { get; private set; }

    public void SetResult(T result)
    {
        if (HasResult)
        {
            throw new InvalidOperationException("Result already set");
        }

        this.result = result;
        HasResult = true;

        subject.OnNext(result);
        subject.OnCompleted();
    }

    public IDisposable Subscribe(IObserver<T> observer)
        => subject.Subscribe(observer);
}
