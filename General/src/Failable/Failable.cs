namespace Markwardt;

public static class FailableExtensions
{
    public static Failable Nest(this Failable failable, string message)
        => failable.Exception == null ? failable : new(new InvalidOperationException(message, failable.Exception));

    public static Failable<T> Nest<T>(this Failable<T> failable, string message)
        => failable.Exception == null ? failable : new(new InvalidOperationException(message, failable.Exception));

    public static Failable<T> AsFailable<T>(this T result)
        => Failable.Success(result);

    public static Failable AsFailable(this Exception exception)
        => new(exception);

    public static Failable<T> AsFailable<T>(this Exception exception)
        => new(exception);

    public static Failable AsFailable(this Exception exception, string message)
        => exception.AsFailable().Nest(message);

    public static Failable<T> AsFailable<T>(this Exception exception, string message)
        => exception.AsFailable<T>().Nest(message);
}

public class Failable
{
    private static readonly Failable success = new(null);

    public static implicit operator Task<Failable>(Failable failable)
        => Task.FromResult(failable);

    public static implicit operator ValueTask<Failable>(Failable failable)
        => ValueTask.FromResult(failable);

    public static implicit operator Failable(Exception exception)
        => new(exception);

    public static Failable Success()
        => success;

    public static Failable<T> Success<T>(T result)
        => new(result);

    public static Failable Fail(Exception exception)
        => new(exception);

    public static Failable<T> Fail<T>(Exception exception)
        => new(exception);

    internal Failable(Exception? exception)
        => Exception = exception;

    public Exception? Exception { get; }

    public void Verify()
    {
        if (Exception != null)
        {
            throw Exception;
        }
    }

    public void Deconstruct(out bool isFailed, out Exception exception)
    {
        isFailed = Exception != null;
        exception = Exception!;
    }

    public override string ToString()
        => Exception != null ? $"Failure ({Exception.Message})" : "Success";
}

public class Failable<T> : Failable
{
    public static implicit operator Task<Failable<T>>(Failable<T> failable)
        => Task.FromResult(failable);
        
    public static implicit operator ValueTask<Failable<T>>(Failable<T> failable)
        => ValueTask.FromResult(failable);

    public static implicit operator Failable<T>(T value)
        => new(value);

    public static implicit operator Failable<T>(Exception exception)
        => new(exception);

    internal Failable(T result)
        : base(null)
        => this.result = result;

    internal Failable(Exception exception)
        : base(exception)
        => result = default!;

    private readonly T result;

    public T Result => Exception == null ? result : throw Exception;

    public void Deconstruct(out bool isFailed, out Exception exception, out T result)
    {
        isFailed = Exception != null;
        exception = Exception!;
        result = this.result!;
    }

    public Failable<TConverted> Convert<TConverted>(Func<T, Failable<TConverted>> convert)
    {
        if (Exception != null)
        {
            return Exception;
        }

        Failable<TConverted> tryConvert = convert(Result);
        if (tryConvert.Exception != null)
        {
            return tryConvert.Exception;
        }

        return tryConvert.Result;
    }

    public Failable<TConverted> Convert<TConverted>(Func<T, TConverted> convert)
        => Convert(r => Success(convert(r)));

    public async ValueTask<Failable<TConverted>> Convert<TConverted>(Func<T, ValueTask<Failable<TConverted>>> convert)
    {
        if (Exception != null)
        {
            return Exception;
        }

        Failable<TConverted> tryConvert = await convert(Result);
        if (tryConvert.Exception != null)
        {
            return tryConvert.Exception;
        }

        return tryConvert.Result;
    }

    public async ValueTask<Failable<TConverted>> Convert<TConverted>(Func<T, ValueTask<TConverted>> convert)
        => await Convert(async r => Success(await convert(r)));

    public Failable<TCasted> Cast<TCasted>()
        => Convert(r => (TCasted)(object?)r!);

    public override string ToString()
        => Exception != null ? $"Failure ({Exception.GetRecursiveMessage()})" : Result?.ToString() ?? "null";
}