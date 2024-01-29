namespace Markwardt;

public interface IFileNode
{
    string Name { get; }
    string FullName { get; }

    IFolder Ascend();

    IFileNode AsNode();
    IFile AsFile();
    IFolder AsFolder();

    ValueTask<Failable<bool>> Exists(CancellationToken cancellation = default);
    ValueTask<Failable> Delete(bool verify = true, CancellationToken cancellation = default);
}

public static class FileNodeExtensions
{
    public static IFolder Ascend(this IFileNode node, int ascensions)
    {
        IFolder parent = node.Ascend();
        for (int i = 1; i < ascensions; i++)
        {
            node = node.Ascend();
        }

        return parent;
    }
    
    public static IFolder AscendRoot(this IFileNode node)
    {
        while (true)
        {
            IFolder parent = node.Ascend();
            if (parent == node)
            {
                return parent;
            }

            node = parent;
        }
    }

    public static string? GetLocalPath(this IFileNode node)
        => node is LocalFileNode ? node.FullName : null;
}