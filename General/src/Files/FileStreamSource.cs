namespace Markwardt;

public static class FileStreamSourceExtensions
{
    public static IStreamSource AsSource(this IFile file, FileOpenMode mode, bool readOnly)
        => new FileStreamSource(file, mode, readOnly);

    public static IStreamSource AsReadSource(this IFile file)
        => file.AsSource(FileOpenMode.Open, true);

    public static IStreamSource AsAppendSource(this IFile file)
        => file.AsSource(FileOpenMode.Append, false);

    public static IStreamSource AsOverwriteSource(this IFile file)
        => file.AsSource(FileOpenMode.Overwrite, false);
}

public class FileStreamSource(IFile file, FileOpenMode mode, bool readOnly) : IStreamSource
{
    public async ValueTask<Failable<Stream>> Open(CancellationToken cancellation = default)
        => await file.Open(mode, readOnly, cancellation);
}