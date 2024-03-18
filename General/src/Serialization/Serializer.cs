namespace Markwardt;

public interface ISerializer<in T>
{
    ValueTask<Failable> Serialize(T data, Stream output, CancellationToken cancellation = default);
}