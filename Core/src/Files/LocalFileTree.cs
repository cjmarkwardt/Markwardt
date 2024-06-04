namespace Markwardt;

[ServiceType<IFileTree>]
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

                    if (tryDescendFolder.Exception is OperationCanceledException)
                    {
                        yield break;
                    }
                }
                else
                {
                    await foreach (Failable<IFile> tryDescendFile in tryDescendFolder.Result.DescendAllFiles(true, cancellation))
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
                            yield return tryDescendFile.Result.AsFailable();
                        }
                    }
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
                await foreach (Failable<IFolder> tryDescendFolder in folder.DescendAllFolders(cancellation: cancellation))
                {
                    if (tryDescendFolder.Exception != null)
                    {
                        yield return tryDescendFolder;
                    
                        if (tryDescendFolder.Exception is OperationCanceledException)
                        {
                            yield break;
                        }
                    }
                    else
                    {
                        yield return tryDescendFolder.Result.AsFailable();
                    }
                }
            }
        }
    }
}