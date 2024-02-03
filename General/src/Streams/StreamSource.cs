namespace Markwardt;

public interface IStreamSource
{
    ValueTask<Failable<Stream>> Open(CancellationToken cancellation = default);
}

public static class StreamSourceExtensions
{
    public static async ValueTask<Failable<TResult>> Operate<TResult>(this IStreamSource source, AsyncFunc<Stream, TResult> operate, CancellationToken cancellation = default)
    {
        Failable<Stream> tryOpen = await source.Open(cancellation);
        if (tryOpen.Exception != null)
        {
            return tryOpen.Exception;
        }

        using Stream stream = tryOpen.Result;

        Failable<TResult> tryOperate = await operate(stream, cancellation);
        if (tryOperate.Exception != null)
        {
            return tryOperate.Exception;
        }

        return tryOperate.Result;
    }

    public static async ValueTask<Failable> Operate(this IStreamSource source, AsyncAction<Stream> action, CancellationToken cancellation = default)
        => await source.Operate<bool>(async (stream, cancellation) =>
        {
            Failable tryAction = await action(stream, cancellation);
            if (tryAction.Exception != null)
            {
                return tryAction.Exception;
            }

            return true;
        }, cancellation);

    public static async ValueTask<Failable> Write(this IStreamSource source, ReadOnlyMemory<byte> data, CancellationToken cancellation = default)
        => await source.Operate(async (stream, _) => await Failable.GuardAsync(async () => await stream.WriteAsync(data, cancellation)), cancellation);

    public static async ValueTask<Failable> WriteText(this IStreamSource source, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await source.Write((encoding ?? Encoding.UTF8).GetBytes(text), cancellation);

    public static async ValueTask<Failable<ReadOnlyMemory<byte>>> Read(this IStreamSource source, CancellationToken cancellation = default)
        => await source.Operate(async (stream, cancellation) =>
        {
            using MemoryStream buffer = new();
            Failable tryCopy = await Failable.GuardAsync(async () => await stream.CopyToAsync(buffer, cancellation));
            if (tryCopy.Exception != null)
            {
                return tryCopy.Exception;
            }

            return Failable.Success<ReadOnlyMemory<byte>>(buffer.ToArray());
        }, cancellation);

    public static async ValueTask<Failable<string>> ReadText(this IStreamSource source, Encoding? encoding = null, CancellationToken cancellation = default)
    {
        Failable<ReadOnlyMemory<byte>> tryRead = await source.Operate(async (stream, cancellation) =>
        {
            using MemoryStream buffer = new();
            Failable tryCopy = await Failable.GuardAsync(async () => await stream.CopyToAsync(buffer, cancellation));
            if (tryCopy.Exception != null)
            {
                return tryCopy.Exception;
            }

            if (encoding is null)
            {
                Failable<Encoding> tryDetect = await buffer.DetectEncoding(Encoding.UTF8);
                if (tryDetect.Exception != null)
                {
                    return tryDetect.Exception;
                }
                
                encoding = tryDetect.Result;
            }

            return Failable.Success<ReadOnlyMemory<byte>>(buffer.ToArray());
        }, cancellation);

        if (tryRead.Exception != null)
        {
            return tryRead.Exception;
        }

        return (encoding ?? Encoding.UTF8).GetString(tryRead.Result.ToArray());
    }
}