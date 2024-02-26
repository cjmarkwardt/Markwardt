namespace Markwardt;

public static class FolderStreamSourceExtensions
{
    public static IMultiStreamSource AsSource(this IFolder folder, FileOpenMode mode, bool readOnly)
        => new FolderStreamSource(folder, mode, readOnly);

    public static IMultiStreamSource AsReadSource(this IFolder folder)
        => folder.AsSource(FileOpenMode.Open, true);

    public static IMultiStreamSource AsAppendSource(this IFolder folder)
        => folder.AsSource(FileOpenMode.Append, false);

    public static IMultiStreamSource AsOverwriteSource(this IFolder folder)
        => folder.AsSource(FileOpenMode.Overwrite, false);
}

public class FolderStreamSource(IFolder folder, FileOpenMode mode, bool readOnly) : IMultiStreamSource
{
    public IStreamSource Get(string id)
        => folder.Descend(id).AsFile().AsSource(mode, readOnly);
}