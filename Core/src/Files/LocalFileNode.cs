using Microsoft.VisualBasic.FileIO;

using SearchOption = System.IO.SearchOption;

namespace Markwardt;

public record LocalFileNode(string Path) : IFile, IFolder
{
    public string Name => System.IO.Path.GetFileName(Path).TrimEnd('/', '\\');
    public string FullName => Path.TrimEnd('/', '\\');

    public IFolder Ascend()
    {
        string? parent = System.IO.Path.GetDirectoryName(Path);
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

    public async ValueTask<Failable> Move(IFileNode newNode, bool overwrite = false, CancellationToken cancellation = default)
    {
        Failable<bool> tryExists = await FileExists();
        if (tryExists.Exception != null)
        {
            return tryExists.Exception;
        }

        if (cancellation.IsCancellationRequested)
        {
            return Failable.Cancel();
        }

        if (tryExists.Result)
        {
            return await FileMove(newNode.AsFile(), overwrite);
        }
        else
        {
            return await FolderMove(newNode.AsFolder(), overwrite);
        }
    }

    public async ValueTask<Failable> Copy(IFileNode newNode, bool overwrite = false, CancellationToken cancellation = default)
    {
        Failable<bool> tryExists = await FileExists();
        if (tryExists.Exception != null)
        {
            return tryExists.Exception;
        }

        if (cancellation.IsCancellationRequested)
        {
            return Failable.Cancel();
        }

        if (tryExists.Result)
        {
            return await FileCopy(newNode.AsFile(), overwrite);
        }
        else
        {
            return await FolderCopy(newNode.AsFolder(), overwrite);
        }
    }

    public override string ToString()
        => FullName;

    async ValueTask<Failable<bool>> IFile.Exists(CancellationToken cancellation)
        => await FileExists();

    async ValueTask<Failable> IFile.Create(bool verify, CancellationToken cancellation)
        => await ExecuteAndCheck(NodeType.File, () => File.Create(Path).Dispose(), verify, true, $"File creation failed for {Path}", cancellation);

    async ValueTask<Failable> IFile.Delete(bool verify, CancellationToken cancellation)
        => await FileDelete(verify, cancellation);

    async ValueTask<Failable> IFile.Move(IFile newFile, bool overwrite, CancellationToken cancellation)
        => await FileMove(newFile, overwrite);

    async ValueTask<Failable> IFile.Copy(IFile newFile, bool overwrite, CancellationToken cancellation)
        => await FileCopy(newFile, overwrite);

    async ValueTask<Failable<long>> IFile.GetLength(CancellationToken cancellation)
        => await Execute(() => new FileInfo(Path).Length);

    async ValueTask<Failable<Stream>> IFile.Open(FileOpenMode mode, bool readOnly, CancellationToken cancellation)
    {
        Failable<FileMode> openMode = GetLocalMode(mode);
        if (openMode.Exception != null)
        {
            return openMode.Exception;
        }

        Failable<FileStream> tryOpen = await Execute(() => File.Open(Path, openMode.Result, readOnly ? FileAccess.Read : FileAccess.ReadWrite, FileShare.ReadWrite));
        if (tryOpen.Exception != null)
        {
            return tryOpen.Exception;
        }

        FileStream stream = tryOpen.Result;

        if (cancellation.IsCancellationRequested)
        {
            await stream.DisposeAsync();
            return Failable.Cancel<Stream>();
        }

        if (mode is FileOpenMode.Overwrite)
        {
            Failable tryTruncate = await Execute(() => Failable.Guard(() => stream.SetLength(0)));
            if (tryTruncate.Exception != null)
            {
                return tryTruncate.Exception;
            }
        }
        else if (mode is FileOpenMode.Append)
        {
            Failable trySeek = await Execute(() => stream.Seek(0, SeekOrigin.End));
            if (trySeek.Exception != null)
            {
                return trySeek.Exception;
            }
        }

        return tryOpen.Result;
    }

    async ValueTask<Failable<DateTime>> IFile.GetLastAccessTime(CancellationToken cancellation)
        => await Execute(() => File.GetLastAccessTime(Path));

    async ValueTask<Failable> IFile.SetLastAccessTime(DateTime time, CancellationToken cancellation)
        => await Execute(() => File.SetLastAccessTime(Path, time));

    async ValueTask<Failable<DateTime>> IFile.GetLastWriteTime(CancellationToken cancellation)
        => await Execute(() => File.GetLastWriteTime(Path));

    async ValueTask<Failable> IFile.SetLastWriteTime(DateTime time, CancellationToken cancellation)
        => await Execute(() => File.SetLastWriteTime(Path, time));

    async ValueTask<Failable<DateTime>> IFile.GetCreationTime(CancellationToken cancellation)
        => await Execute(() => File.GetCreationTime(Path));

    async ValueTask<Failable> IFile.SetCreationTime(DateTime time, CancellationToken cancellation)
        => await Execute(() => File.SetCreationTime(Path, time));

    async ValueTask<Failable<bool>> IFolder.Exists(CancellationToken cancellation)
        => await FolderExists();

    async ValueTask<Failable> IFolder.Create(bool verify, CancellationToken cancellation)
        => await ExecuteAndCheck(NodeType.Folder, () => Directory.CreateDirectory(Path), verify, true, $"Folder creation failed for {Path}", cancellation);

    async ValueTask<Failable> IFolder.Delete(bool verify, CancellationToken cancellation)
        => await FolderDelete(verify, cancellation);

    async ValueTask<Failable> IFolder.Move(IFolder newFolder, bool overwrite, CancellationToken cancellation)
        => await FolderMove(newFolder, overwrite);

    async ValueTask<Failable> IFolder.Copy(IFolder newFolder, bool overwrite, CancellationToken cancellation)
        => await FolderCopy(newFolder, overwrite);

    IFileNode IFileTree.Descend(string name)
        => new LocalFileNode(System.IO.Path.Combine(Path, name));

    IAsyncEnumerable<Failable<IFile>> IFileTree.DescendAllFiles(bool recursive, CancellationToken cancellation)
        => DescendAll<IFile>(Directory.EnumerateFiles, recursive, p => new LocalFileNode(p));

    IAsyncEnumerable<Failable<IFolder>> IFileTree.DescendAllFolders(bool recursive, CancellationToken cancellation)
        => DescendAll<IFolder>(Directory.EnumerateDirectories, recursive, p => new LocalFileNode(p));

    private async ValueTask<Failable<bool>> FileExists()
        => await Execute(() => File.Exists(Path));

    private async ValueTask<Failable> FileMove(IFile newFile, bool overwrite)
        => await Execute(newFile, targetPath => FileSystem.MoveFile(Path, targetPath, overwrite));

    private async ValueTask<Failable> FileCopy(IFile newFile, bool overwrite)
        => await Execute(newFile, targetPath => FileSystem.CopyFile(Path, targetPath, overwrite));

    private async ValueTask<Failable> FileDelete(bool verify, CancellationToken cancellation = default)
        => await ExecuteAndCheck(NodeType.File, () => File.Delete(Path), verify, false, $"File deletion failed for {Path}", cancellation);
    
    private async ValueTask<Failable<bool>> FolderExists()
        => await Execute(() => Directory.Exists(Path));

    private async ValueTask<Failable> FolderDelete(bool verify, CancellationToken cancellation = default)
        => await ExecuteAndCheck(NodeType.Folder, () => Directory.Delete(Path), verify, false, $"Folder deletion failed for {Path}", cancellation);

    private async ValueTask<Failable> FolderMove(IFolder newFolder, bool overwrite)
        => await Execute(newFolder, targetPath => FileSystem.MoveDirectory(Path, targetPath, overwrite));

    private async ValueTask<Failable> FolderCopy(IFolder newFolder, bool overwrite)
        => await Execute(newFolder, targetPath => FileSystem.CopyDirectory(Path, targetPath, overwrite));

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

    private async ValueTask<Failable> Execute(Action action)
        => await Task.Run(() => Failable.Guard(action));

    private async ValueTask<Failable<T>> Execute<T>(Func<T> action)
        => await Task.Run(() => Failable.Guard(action));

    private async ValueTask<Failable> Execute(IFileNode target, Action<string> action)
    {
        Failable<string> tryGetPath = target.GetLocalPath();
        if (tryGetPath.Exception != null)
        {
            return tryGetPath.Exception;
        }

        return await Execute(() => action(tryGetPath.Result));
    }

    private async ValueTask<Failable> ExecuteAndCheck(NodeType type, Action action, bool verify, bool exists, string failure, CancellationToken cancellation)
    {
        Failable tryCreate = await Execute(action);
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

        IEnumerator<string> files = enumerate(Path, "*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).GetEnumerator();
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

    private Failable<FileMode> GetLocalMode(FileOpenMode mode)
        => mode switch
        {
            FileOpenMode.Open => FileMode.Open,
            FileOpenMode.Create => FileMode.CreateNew,
            FileOpenMode.OpenOrCreate => FileMode.OpenOrCreate,
            FileOpenMode.Overwrite or FileOpenMode.Append => FileMode.Create,
            _ => new NotSupportedException(mode.ToString())
        };

    private enum NodeType
    {
        File,
        Folder
    }
}