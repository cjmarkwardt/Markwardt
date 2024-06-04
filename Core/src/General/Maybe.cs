namespace Markwardt;

public static class MaybeExtensions
{
    public static Maybe<T> Maybe<T>(this T value)
        => new(value);

    public static Maybe<T> WhereNotNull<T>(this Maybe<T?> maybe)
        where T : class
        => maybe.Where(x => x is not null).Select(x => x!);

    public static Maybe<T> WhereValueNotNull<T>(this Maybe<T?> maybe)
        where T : struct
        => maybe.Where(x => x.HasValue).Select(x => x!.Value);
}

public readonly struct Maybe<T> : IMultiDisposable
{
    public static implicit operator Maybe<T>(T value)
        => new(value);

    public Maybe()
    {
        value = default!;
        HasValue = false;
    }

    public Maybe(T value)
    {
        this.value = value;
        HasValue = true;
    }

    private readonly T value;

    public readonly bool HasValue { get; }

    public readonly T Value => HasValue ? value : throw new InvalidOperationException("Has no value");
    
    public bool TryGetValue([MaybeNullWhen(false)] out T value)
    {
        if (HasValue)
        {
            value = Value;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    public Maybe<TSelected> Select<TSelected>(Func<T, TSelected> select)
        => TryGetValue(out T? outValue) ? select(outValue).Maybe() : default;

    public Maybe<TSelected> Select<TSelected>(Func<T, Maybe<TSelected>> select)
        => TryGetValue(out T? outValue) ? select(outValue) : default;

    public Maybe<T> Where(Func<T, bool> where)
        => TryGetValue(out T? outValue) && where(outValue) ? this : default;

    public Maybe<TCasted> Cast<TCasted>()
        => Select(x => (TCasted)(object?)x!);

    public Maybe<TCasted> OfType<TCasted>()
        => Where(x => x is TCasted).Cast<TCasted>();

    public override string ToString()
        => HasValue ? Value?.ToString() ?? "<NULL>" : "<EMPTY>";

    public void Dispose()
    {
        if (HasValue && Value is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (HasValue && Value is IAsyncDisposable disposable)
        {
            await disposable.DisposeAsync();
        }
    }
}