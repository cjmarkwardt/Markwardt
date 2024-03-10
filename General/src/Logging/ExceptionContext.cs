namespace Markwardt;

public interface IExceptionContext
{
    IObservable<Exception> ExceptionPushed { get; }

    void PushException(Exception exception);
}