namespace Markwardt;

public static class StreamExtensions
{
    public static async ValueTask<Failable<Encoding>> DetectEncoding(this Stream stream, Encoding fallback)
    {
        using StreamReader reader = new(stream, fallback, true, -1, true);
        Failable tryPeek = await Task.Run(() => Failable.Guard(() => reader.Peek()));
        if (tryPeek.Exception != null)
        {
            return tryPeek.Exception;
        }

        return reader.CurrentEncoding;
    }

    public static async ValueTask<Failable<byte[]>> CopyToArray(this Stream stream, CancellationToken cancellation = default)
    {
        using MemoryStream buffer = new();
        Failable tryCopy = await Failable.GuardAsync(async () => await stream.CopyToAsync(buffer, cancellation));
        if (tryCopy.Exception != null)
        {
            return tryCopy.Exception;
        }

        return buffer.ToArray();
    }

    public static IDisposable<Stream> Buffer(this IDisposable<Stream> stream)
    {
        MemoryStream buffer = new();
        stream.Value.CopyTo(buffer);
        buffer.Position = 0;
        return new Disposable<Stream>(buffer, [stream]);
    }
}