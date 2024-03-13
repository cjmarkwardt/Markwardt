namespace Markwardt;

public interface IMaybe<out T> : IMultiDisposable
{
    bool HasValue { get; }
    T Value { get; }

    IMaybe<TSelected> Select<TSelected>(Func<T, TSelected> select)
        => this.Select<T, TSelected>(select);

    IMaybe<TSelected> Select<TSelected>(Func<T, IMaybe<TSelected>> select)
        => this.Select<T, TSelected>(select);

    IMaybe<TCasted> Cast<TCasted>()
        => this.Cast<T, TCasted>();

    IMaybe<TCasted> OfType<TCasted>()
        => this.OfType<T, TCasted>();
}

public static class MaybeExtensions
{
    public static IMaybe<T> AsMaybe<T>(this T value)
        => new Maybe<T>(value);

    public static bool TryGetValue<T>(this IMaybe<T> maybe, out T value)
    {
        if (maybe.HasValue)
        {
            value = maybe.Value;
            return true;
        }
        else
        {
            value = default!;
            return false;
        }
    }

    public static IMaybe<TSelected> Select<T, TSelected>(this IMaybe<T> maybe, Func<T, TSelected> select)
        => maybe.TryGetValue(out T value) ? select(value).AsMaybe() : Maybe<TSelected>.Empty();

    public static IMaybe<TSelected> Select<T, TSelected>(this IMaybe<T> maybe, Func<T, IMaybe<TSelected>> select)
        => maybe.TryGetValue(out T value) ? select(value) : Maybe<TSelected>.Empty();

    public static IMaybe<T> Where<T>(this IMaybe<T> maybe, Func<T, bool> where)
        => maybe.TryGetValue(out T value) && where(value) ? maybe : Maybe<T>.Empty();

    public static IMaybe<T> WhereNotNull<T>(this IMaybe<T?> maybe)
        where T : class
        => maybe.Where(x => x is not null).Select(x => x!);

    public static IMaybe<T> WhereValueNotNull<T>(this IMaybe<T?> maybe)
        where T : struct
        => maybe.Where(x => x.HasValue).Select(x => x!.Value);

    public static IMaybe<TCasted> Cast<T, TCasted>(this IMaybe<T> maybe)
        => maybe.Select(x => (TCasted)(object?)x!);

    public static IMaybe<TCasted> OfType<T, TCasted>(this IMaybe<T> maybe)
        => maybe.Where(x => x is TCasted).Cast<T, TCasted>();
}

public readonly struct Maybe<T> : IMaybe<T>
{
    public static IMaybe<T> Empty()
        => new Maybe<T>();

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