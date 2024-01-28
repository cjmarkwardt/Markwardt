namespace Markwardt;

public static class FileStreamSourceExtensions
{
    public static IStreamSource AsStreamSource(this IFile file, FileOpenMode mode = FileOpenMode.OpenOrCreate, bool readOnly = false)
        => new FileStreamSource(file, mode, readOnly);
}

public class FileStreamSource(IFile file, FileOpenMode mode, bool readOnly) : IStreamSource
{
    public async ValueTask<Failable<Stream>> Open(CancellationToken cancellation = default)
        => await file.Open(mode, readOnly, cancellation);
}