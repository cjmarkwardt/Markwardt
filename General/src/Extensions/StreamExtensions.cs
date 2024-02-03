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
}