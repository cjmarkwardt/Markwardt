namespace Markwardt;

public interface IFailureContext
{
    IObservable<Failable> FailReported { get; }

    void Fail(Failable failable);
}

public static class FailureContextExtensions
{
    public static void Fail(this IFailureContext context, Exception exception)
        => context.Fail(Failable.Fail(exception));
}