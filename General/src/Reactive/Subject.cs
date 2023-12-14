namespace Markwardt;

public sealed class Subject : Observer, ISubject<bool>, IObservable, IDisposable
{
    private readonly Subject<bool> subject = new();

    public void Dispose()
        => subject.Dispose();

    public override void OnCompleted()
        => subject.OnCompleted();

    public override void OnError(Exception error)
        => subject.OnError(error);

    public override void OnNext()
        => subject.OnNext(true);

    public IDisposable Subscribe(IObserver<bool> observer)
        => subject.Subscribe(observer);
}
