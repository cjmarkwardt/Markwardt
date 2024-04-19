namespace Markwardt;

public interface IStreamSource
{
    string? TypeHint { get; }

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

    public static async ValueTask<Failable> Write<T>(this IStreamSource source, IStreamSerializer<T> serializer, T data, CancellationToken cancellation = default)
        => await source.Operate(async (stream, _) => await serializer.Serialize(data, stream, cancellation), cancellation);

    public static async ValueTask<Failable> WriteText(this IStreamSource source, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await source.Write((encoding ?? Encoding.UTF8).GetBytes(text), cancellation);

    public static async ValueTask<Failable<byte[]>> Read(this IStreamSource source, CancellationToken cancellation = default)
        => await source.Operate(async (stream, _) => await stream.CopyToArray(cancellation), cancellation);

    public static async ValueTask<Failable<T>> Read<T>(this IStreamSource source, IStreamDeserializer<T> deserializer, CancellationToken cancellation = default)
        => await source.Operate(async (stream, _) => await deserializer.Deserialize(stream, cancellation), cancellation);

    public static async ValueTask<Failable<string>> ReadText(this IStreamSource source, Encoding? encoding = null, CancellationToken cancellation = default)
    {
        Failable<byte[]> tryRead = await source.Operate<byte[]>(async (stream, cancellation) =>
        {
            if (encoding is null)
            {
                Failable<Encoding> tryDetect = await stream.DetectEncoding(Encoding.UTF8);
                if (tryDetect.Exception != null)
                {
                    return tryDetect.Exception;
                }
                
                encoding = tryDetect.Result;
            }
            
            Failable<byte[]> tryCopy = await stream.CopyToArray(cancellation);
            if (tryCopy.Exception != null)
            {
                return tryCopy.Exception;
            }

            return tryCopy.Result;
        }, cancellation);

        if (tryRead.Exception != null)
        {
            return tryRead.Exception;
        }

        return (encoding ?? Encoding.UTF8).GetString(tryRead.Result.ToArray());
    }
}