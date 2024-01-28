namespace Markwardt;

[ServiceType(typeof(IFileTree))]
public class LocalFileTreeTag : ImplementationTag<LocalFileTree>;

public class LocalFileTree : IFileTree
{
    public IFileNode Descend(string name)
        => new LocalFileNode(name);

    public async IAsyncEnumerable<Failable<IFile>> DescendAllFiles(bool recursive = false, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        if (recursive)
        {
            await foreach (Failable<IFolder> tryDescendFolder in DescendAllFolders(false, cancellation))
            {
                if (tryDescendFolder.Exception != null)
                {
                    yield return tryDescendFolder.Exception;
                    yield break;
                }

                await foreach (Failable<IFile> tryDescendFile in tryDescendFolder.Result.DescendAllFiles(true, cancellation))
                {
                    if (tryDescendFile.Exception != null)
                    {
                        yield return tryDescendFile.Exception;
                        yield break;
                    }

                    yield return tryDescendFile.Result.AsFailable();
                }
            }
        }
    }

    public async IAsyncEnumerable<Failable<IFolder>> DescendAllFolders(bool recursive = false, [EnumeratorCancellation] CancellationToken cancellation = default)
    {
        Failable<IEnumerable<string>> tryGetFolders = Failable.Guard(() => DriveInfo.GetDrives().Select(x => x.RootDirectory.FullName));
        if (tryGetFolders.Exception != null)
        {
            yield return tryGetFolders.Exception.AsFailable<IFolder>();
        }

        foreach (string path in tryGetFolders.Result)
        {
            IFolder folder = new LocalFileNode(path).AsFolder();
            yield return folder.AsFailable();

            if (recursive)
            {
                await foreach (Failable<IFolder> tryDescend in folder.DescendAllFolders(true, cancellation))
                {
                    if (tryDescend.Exception != null)
                    {
                        yield return tryDescend;
                        yield break;
                    }

                    yield return tryDescend.Result.AsFailable();
                }
            }
        }
    }
}