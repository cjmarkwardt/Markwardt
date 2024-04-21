namespace Markwardt;

public interface IFileTree
{
    IFileNode Descend(string name);

    IAsyncEnumerable<Failable<IFile>> DescendAllFiles(bool recursive = false, CancellationToken cancellation = default);
    IAsyncEnumerable<Failable<IFolder>> DescendAllFolders(bool recursive = false, CancellationToken cancellation = default);
}

public static class FileTreeExtensions
{
    public static IFileNode Descend(this IFileTree tree, IEnumerable<string> names)
    {
        IFileNode current = tree.Descend(names.First());
        foreach (string name in names.Skip(1))
        {
            current = current.AsFolder().Descend(name);
        }

        return current;
    }

    public static IFileNode Descend(this IFileTree tree, string name, params string[] chain)
        => tree.Descend(chain.Prepend(name));

    public static async IAsyncEnumerable<Failable<IFileNode>> DescendAll(this IFileTree tree, bool recursive = false, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        await foreach (Failable<IFile> tryDescendFile in tree.DescendAllFiles(recursive, cancellation))
        {
            if (tryDescendFile.Exception != null)
            {
                yield return tryDescendFile.Exception;

                if (tryDescendFile.Exception is OperationCanceledException)
                {
                    yield break;
                }
            }
            else
            {
                yield return tryDescendFile.Cast<IFileNode>();
            }
        }

        if (cancellation.IsCancellationRequested)
        {
            yield return Failable.Cancel<IFileNode>();
            yield break;
        }
        
        await foreach (Failable<IFolder> tryDescendFolder in tree.DescendAllFolders(recursive, cancellation))
        {
            if (tryDescendFolder.Exception != null)
            {
                yield return tryDescendFolder.Exception;

                if (tryDescendFolder.Exception is OperationCanceledException)
                {
                    yield break;
                }
            }
            else
            {
                yield return tryDescendFolder.Cast<IFileNode>();
            }
        }
    }
}