namespace Markwardt;

public interface IStreamSource
{
    ValueTask<Failable<Stream>> Open(CancellationToken cancellation = default);
}

public static class StreamSourceExtensions
{
    public static async ValueTask<Failable<TResult>> Operate<TResult>(this IStreamSource source, AsyncFunc<Stream, Failable<TResult>> action, CancellationToken cancellation = default)
    {
        Failable<Stream> tryOpen = await source.Open(cancellation);
        if (tryOpen.Exception != null)
        {
            return tryOpen.Exception;
        }

        using Stream stream = tryOpen.Result;
        
        Failable<TResult> tryPrepare = await Failable.GuardAsync(async () => await action(stream));
        if (tryPrepare.Exception != null)
        {
            return tryPrepare.Exception;
        }

        return tryPrepare.Result;
    }

    public static async ValueTask<Failable> Operate(this IStreamSource source, AsyncFunc<Stream, Failable> action, CancellationToken cancellation = default)
        => await source.Operate<bool>(async stream =>
        {
            Failable tryAction = await action(stream);
            if (tryAction.Exception != null)
            {
                return tryAction.Exception;
            }

            return true;
        }, cancellation);

    public static async ValueTask<Failable> Truncate(this IStreamSource source, CancellationToken cancellation = default)
        => await source.Operate(stream => { stream.SetLength(0); return Failable.Success(); }, cancellation);

    public static async ValueTask<Failable> Append(this IStreamSource source, ReadOnlyMemory<byte> data, CancellationToken cancellation = default)
        => await source.Operate(async stream => { await stream.WriteAsync(data, cancellation); return Failable.Success(); }, cancellation);

    public static async ValueTask<Failable> AppendText(this IStreamSource source, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await source.Append((encoding ?? Encoding.UTF8).GetBytes(text), cancellation);

    public static async ValueTask<Failable> Overwrite(this IStreamSource source, ReadOnlyMemory<byte> data, CancellationToken cancellation = default)
        => await source.Operate(async stream => { stream.SetLength(0); await stream.WriteAsync(data, cancellation); return Failable.Success(); }, cancellation);

    public static async ValueTask<Failable> OverwriteText(this IStreamSource source, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await source.Overwrite((encoding ?? Encoding.UTF8).GetBytes(text), cancellation);

    public static async ValueTask<Failable<ReadOnlyMemory<byte>>> Read(this IStreamSource source, CancellationToken cancellation = default)
        => await source.Operate(async stream =>
        {
            using MemoryStream buffer = new();
            await stream.CopyToAsync(buffer);
            return Failable.Success<ReadOnlyMemory<byte>>(buffer.ToArray());
        }, cancellation);

    public static async ValueTask<Failable<string>> ReadText(this IStreamSource source, Encoding? encoding = null, CancellationToken cancellation = default)
    {
        Failable<ReadOnlyMemory<byte>> tryRead = await source.Operate(async stream =>
        {
            using MemoryStream buffer = new();
            await stream.CopyToAsync(buffer);

            if (encoding is null)
            {
                using StreamReader reader = new(buffer, Encoding.UTF8, true);
                reader.Peek();
                encoding = reader.CurrentEncoding;
            }

            return Failable.Success<ReadOnlyMemory<byte>>(buffer.ToArray());
        }, cancellation);

        if (tryRead.Exception != null)
        {
            return tryRead.Exception;
        }

        return (encoding ?? Encoding.Unicode).GetString(tryRead.Result.ToArray());
    }
}