namespace Markwardt;

public class LocalFileNode(string path) : IFile, IFolder
{
    public string Name => Path.GetFileName(path).TrimEnd('/', '\\');
    public string FullName => path.TrimEnd('/', '\\');

    public IFolder Ascend()
    {
        string? parent = Path.GetDirectoryName(path);
        if (parent == null)
        {
            return this;
        }

        return new LocalFileNode(parent);
    }

    public IFile AsFile()
        => this;

    public IFolder AsFolder()
        => this;

    public IFileNode AsNode()
        => this;

    public async ValueTask<Failable<bool>> Exists(CancellationToken cancellation = default)
    {
        Failable<bool> tryFileExists = await FileExists();
        if (tryFileExists.Exception != null)
        {
            return tryFileExists.Exception;
        }

        if (tryFileExists.Result)
        {
            return true;
        }

        if (cancellation.IsCancellationRequested)
        {
            return Failable.Cancel<bool>();
        }

        Failable<bool> tryFolderExists = await FolderExists();
        if (tryFolderExists.Exception != null)
        {
            return tryFolderExists.Exception;
        }

        return tryFolderExists.Result;
    }

    public async ValueTask<Failable> Delete(bool verify = true, CancellationToken cancellation = default)
    {
        Failable tryFileDelete = await FileDelete(verify, cancellation);
        if (tryFileDelete.Exception != null)
        {
            return tryFileDelete.Exception;
        }

        if (cancellation.IsCancellationRequested)
        {
            return Failable.Cancel();
        }

        Failable tryFolderDelete = await FolderDelete(verify, cancellation);
        if (tryFolderDelete.Exception != null)
        {
            return tryFolderDelete.Exception;
        }

        return Failable.Success();
    }

    async ValueTask<Failable<bool>> IFile.Exists(CancellationToken cancellation)
        => await FileExists();

    async ValueTask<Failable> IFile.Create(bool verify, CancellationToken cancellation)
        => await Execute(NodeType.File, () => File.Create(path).Dispose(), verify, true, $"File creation failed for {path}", cancellation);

    async ValueTask<Failable> IFile.Delete(bool verify, CancellationToken cancellation)
        => await FileDelete(verify, cancellation);

    async ValueTask<Failable<Stream>> IFile.Open(FileOpenMode mode, bool readOnly, CancellationToken cancellation)
    {
        FileMode openMode = FileMode.Open;
        if (mode is FileOpenMode.Create)
        {
            openMode = FileMode.CreateNew;
        }
        else if (mode is FileOpenMode.OpenOrCreate)
        {
            openMode = FileMode.Create;
        }

        Failable<FileStream> tryOpen = await Task.Run(() => Failable.Guard(() => File.Open(path, openMode, readOnly ? FileAccess.Read : FileAccess.ReadWrite, FileShare.ReadWrite)));
        if (tryOpen.Exception != null)
        {
            return tryOpen.Exception.AsFailable<Stream>($"Failed to open {path}");
        }

        return tryOpen.Result;
    }

    async ValueTask<Failable<bool>> IFolder.Exists(CancellationToken cancellation)
        => await FolderExists();

    async ValueTask<Failable> IFolder.Create(bool verify, CancellationToken cancellation)
        => await Execute(NodeType.Folder, () => Directory.CreateDirectory(path), verify, true, $"Folder creation failed for {path}", cancellation);

    async ValueTask<Failable> IFolder.Delete(bool verify, CancellationToken cancellation)
        => await FolderDelete(verify, cancellation);

    IFileNode IFileTree.Descend(string name)
        => new LocalFileNode(Path.Combine(path, name));

    IAsyncEnumerable<Failable<IFile>> IFileTree.DescendAllFiles(bool recursive, CancellationToken cancellation)
        => DescendAll<IFile>(Directory.EnumerateFiles, recursive, p => new LocalFileNode(p));

    IAsyncEnumerable<Failable<IFolder>> IFileTree.DescendAllFolders(bool recursive, CancellationToken cancellation)
        => DescendAll<IFolder>(Directory.EnumerateDirectories, recursive, p => new LocalFileNode(p));

    private async ValueTask<Failable<bool>> FileExists()
        => await Task.Run(() => Failable.Guard(() => File.Exists(path)));

    private async ValueTask<Failable<bool>> FolderExists()
        => await Task.Run(() => Failable.Guard(() => Directory.Exists(path)));

    private async ValueTask<Failable> FileDelete(bool verify, CancellationToken cancellation = default)
        => await Execute(NodeType.File, () => File.Delete(path), verify, false, $"File deletion failed for {path}", cancellation);

    private async ValueTask<Failable> FolderDelete(bool verify, CancellationToken cancellation = default)
        => await Execute(NodeType.Folder, () => Directory.Delete(path), verify, false, $"Folder deletion failed for {path}", cancellation);

    private async ValueTask<Failable> WaitForExists(NodeType node, bool exists, CancellationToken cancellation)
        => await TaskExtensions.WaitFor(async cancellation =>
        {
            Failable<bool> tryExists = node is NodeType.File ? await FileExists() : await FolderExists();
            if (tryExists.Exception != null)
            {
                return tryExists.Exception;
            }

            return exists ? tryExists.Result : !tryExists.Result;
        }, TimeSpan.FromMilliseconds(5), TimeSpan.FromSeconds(0.5), cancellation);

    private async ValueTask<Failable> Execute(NodeType type, Action action, bool verify, bool exists, string failure, CancellationToken cancellation)
    {
        Failable tryCreate = await Task.Run(() => Failable.Guard(action));
        if (tryCreate.Exception != null)
        {
            return tryCreate.Exception.AsFailable(failure);
        }

        if (cancellation.IsCancellationRequested)
        {
            return Failable.Cancel();
        }

        if (verify)
        {
            Failable tryWait = await WaitForExists(type, exists, cancellation);
            if (tryWait.Exception != null)
            {
                return tryWait.Exception.AsFailable(failure);
            }
        }

        return Failable.Success();
    }

    private async IAsyncEnumerable<Failable<T>> DescendAll<T>(Func<string, string, SearchOption, IEnumerable<string>> enumerate, bool recursive, Func<string, T> create)
    {
        await Task.CompletedTask;

        IEnumerator<string> files = enumerate(path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).GetEnumerator();
        while (true)
        {
            Failable<string> tryNext;
            try
            {
                if (!files.MoveNext())
                {
                    yield break;
                }

                tryNext = files.Current;
            }
            catch (Exception exception)
            {
                tryNext = exception.AsFailable<string>();
            }

            if (tryNext.Exception != null)
            {
                yield return tryNext.Exception;
            }
            else
            {
                yield return create(tryNext.Result);
            }
        }
    }

    private enum NodeType
    {
        File,
        Folder
    }
}