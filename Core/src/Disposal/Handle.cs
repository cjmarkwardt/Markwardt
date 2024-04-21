namespace Markwardt;

public interface IHandle<out T> : IDisposable
{
    T Value { get; }
    bool IsValid { get; }
}

public class Handle<T>(T value, Action<T> release) : IHandle<T>
{
    ~Handle()
        => Dispose();

    private readonly T value = value;
    private readonly Action<T> release = release;

    public bool IsValid { get; private set; } = true;

    public T Value => IsValid ? value : throw new ObjectDisposedException(GetType().Name);

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (IsValid)
        {
            IsValid = false;
            release(value);
        }
    }
}
