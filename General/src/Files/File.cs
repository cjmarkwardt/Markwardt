namespace Markwardt;

public interface IFile : IFileNode
{
    new ValueTask<Failable<bool>> Exists(CancellationToken cancellation = default);
    ValueTask<Failable> Create(bool verify = true, CancellationToken cancellation = default);
    new ValueTask<Failable> Delete(bool verify = true, CancellationToken cancellation = default);

    ValueTask<Failable> Move(IFile newFile, bool overwrite = false, CancellationToken cancellation = default);
    ValueTask<Failable> Copy(IFile newFile, bool overwrite = false, CancellationToken cancellation = default);

    ValueTask<Failable<long>> GetLength(CancellationToken cancellation = default);
    ValueTask<Failable<Stream>> Open(FileOpenMode mode, bool readOnly, CancellationToken cancellation = default);

    ValueTask<Failable<DateTime>> GetLastAccessTime(CancellationToken cancellation = default);
    ValueTask<Failable> SetLastAccessTime(DateTime time, CancellationToken cancellation = default);
    ValueTask<Failable<DateTime>> GetLastWriteTime(CancellationToken cancellation = default);
    ValueTask<Failable> SetLastWriteTime(DateTime time, CancellationToken cancellation = default);
    ValueTask<Failable<DateTime>> GetCreationTime(CancellationToken cancellation = default);
    ValueTask<Failable> SetCreationTime(DateTime time, CancellationToken cancellation = default);
}

public static class FileExtensions
{
    public static async ValueTask<Failable<TResult>> Operate<TResult>(this IFile file, FileOpenMode mode, bool readOnly, AsyncFunc<Stream, TResult> action, CancellationToken cancellation = default)
        => await file.AsSource(mode, readOnly).Operate(action, cancellation);

    public static async ValueTask<Failable> Operate(this IFile file, FileOpenMode mode, bool readOnly, AsyncAction<Stream> action, CancellationToken cancellation = default)
        => await file.AsSource(mode, readOnly).Operate(action, cancellation);

    public static async ValueTask<Failable> Write(this IFile file, FileOpenMode mode, ReadOnlyMemory<byte> data, CancellationToken cancellation = default)
        => await file.AsSource(mode, false).Write(data, cancellation);

    public static async ValueTask<Failable> Write<T>(this IFile file, FileOpenMode mode, ISerializer<T> serializer, T data, CancellationToken cancellation = default)
        => await file.AsSource(mode, false).Write(serializer, data, cancellation);

    public static async ValueTask<Failable> WriteText(this IFile file, FileOpenMode mode, bool readOnly, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await file.AsSource(mode, readOnly).WriteText(text, encoding, cancellation);

    public static async ValueTask<Failable> Append(this IFile file, ReadOnlyMemory<byte> data, CancellationToken cancellation = default)
        => await file.AsAppendSource().Write(data, cancellation);

    public static async ValueTask<Failable> Append<T>(this IFile file, ISerializer<T> serializer, T data, CancellationToken cancellation = default)
        => await file.AsAppendSource().Write(serializer, data, cancellation);

    public static async ValueTask<Failable> AppendText(this IFile file, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await file.AsAppendSource().WriteText(text, encoding, cancellation);

    public static async ValueTask<Failable> Overwrite(this IFile file, ReadOnlyMemory<byte> data, CancellationToken cancellation = default)
        => await file.AsOverwriteSource().Write(data, cancellation);

    public static async ValueTask<Failable> Overwrite<T>(this IFile file, ISerializer<T> serializer, T data, CancellationToken cancellation = default)
        => await file.AsOverwriteSource().Write(serializer, data, cancellation);

    public static async ValueTask<Failable> OverwriteText(this IFile file, string text, Encoding? encoding = null, CancellationToken cancellation = default)
        => await file.AsOverwriteSource().WriteText(text, encoding, cancellation);

    public static async ValueTask<Failable<byte[]>> Read(this IFile file, CancellationToken cancellation = default)
        => await file.AsReadSource().Read(cancellation);

    public static async ValueTask<Failable<T>> Read<T>(this IFile file, IDeserializer<T> deserializer, CancellationToken cancellation = default)
        => await file.AsReadSource().Read(deserializer, cancellation);

    public static async ValueTask<Failable<string>> ReadText(this IFile file, Encoding? encoding = null, CancellationToken cancellation = default)
        => await file.AsReadSource().ReadText(encoding, cancellation);
}