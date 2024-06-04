namespace Markwardt;

public sealed class Finalizer(Action action) : IDisposable
{
    private bool isDisposed;

    ~Finalizer()
        => action();

    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            GC.SuppressFinalize(this);
            action();
        }
    }
}