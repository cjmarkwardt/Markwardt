namespace Markwardt;

public interface IFile : IFileNode
{
    new ValueTask<Failable<bool>> Exists(CancellationToken cancellation = default);
    ValueTask<Failable> Create(bool verify = true, CancellationToken cancellation = default);
    new ValueTask<Failable> Delete(bool verify = true, CancellationToken cancellation = default);

    ValueTask<Failable<Stream>> Open(FileOpenMode mode = FileOpenMode.OpenOrCreate, bool readOnly = false, CancellationToken cancellation = default);
}

public static class FileExtensions
{
    public static async ValueTask<Failable<TResult>> Operate<TResult>(this IFile file, AsyncFunc<Stream, Failable<TResult>> action, FileOpenMode mode = FileOpenMode.OpenOrCreate, bool readOnly = false, CancellationToken cancellation = default)
        => await file.AsStreamSource(mode, readOnly).Operate(action, cancellation);

    public static async ValueTask<Failable> Operate(this IFile file, AsyncFunc<Stream, Failable> action, FileOpenMode mode = FileOpenMode.OpenOrCreate, bool readOnly = false, CancellationToken cancellation = default)
        => await file.AsStreamSource(mode, readOnly).Operate(action, cancellation);

    public static async ValueTask<Failable> Truncate(this IFile file, CancellationToken cancellation = default)
        => await file.AsStreamSource(FileOpenMode.OpenOrCreate).Truncate(cancellation);

    public static async ValueTask<Failable> Append(this IFile file, ReadOnlyMemory<byte> data, CancellationToken cancellation = default)
        => await file.AsStreamSource(FileOpenMode.OpenOrCreate).Append(data, cancellation);

    public static async ValueTask<Failable> AppendText(this IFile file, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await file.AsStreamSource(FileOpenMode.OpenOrCreate).AppendText(text, encoding, cancellation);

    public static async ValueTask<Failable> Overwrite(this IFile file, ReadOnlyMemory<byte> data, CancellationToken cancellation = default)
        => await file.AsStreamSource(FileOpenMode.OpenOrCreate).Overwrite(data, cancellation);

    public static async ValueTask<Failable> OverwriteText(this IFile file, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await file.AsStreamSource(FileOpenMode.OpenOrCreate).OverwriteText(text, encoding, cancellation);

    public static async ValueTask<Failable<ReadOnlyMemory<byte>>> Read(this IFile file, CancellationToken cancellation = default)
        => await file.AsStreamSource(FileOpenMode.Open, true).Read(cancellation);

    public static async ValueTask<Failable<string>> ReadText(this IFile file, Encoding? encoding = null, CancellationToken cancellation = default)
        => await file.AsStreamSource(FileOpenMode.Open, true).ReadText(encoding, cancellation);
}