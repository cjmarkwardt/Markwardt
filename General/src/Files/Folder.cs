namespace Markwardt;

public interface IFolder : IFileNode, IFileTree
{
    new ValueTask<Failable<bool>> Exists(CancellationToken cancellation = default);
    ValueTask<Failable> Create(bool verify = true, CancellationToken cancellation = default);
    new ValueTask<Failable> Delete(bool verify = true, CancellationToken cancellation = default);
}