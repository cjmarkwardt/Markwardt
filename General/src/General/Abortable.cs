namespace Markwardt;

public static class Abortable
{
    public static async ValueTask<Failable<T>> Execute<T>(Func<Failable<T>> action, CancellationToken cancellation = default)
        => await Task.Run(() =>
        {
            Failable<T> result = Failable.Fail<T>("Failed to execute");
            #pragma warning disable SYSLIB0046
            System.Runtime.ControlledExecution.Run(() => result = action(), cancellation);
            #pragma warning restore SYSLIB0046
            return result;
        });

    public static async ValueTask<Failable> Execute(Func<Failable> action, CancellationToken cancellation = default)
        => await Execute(() => action().WithResult(true), cancellation);

    public static async ValueTask<Failable<T>> Execute<T>(Func<T> action, CancellationToken cancellation = default)
        => await Execute(() => Failable.Success(action()), cancellation);

    public static async ValueTask<Failable> Execute(Action action, CancellationToken cancellation = default)
        => await Execute(() => { action(); return Failable.Success(); }, cancellation);
}