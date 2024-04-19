namespace Markwardt;

public interface IStreamSerializer<in T>
{
    ValueTask<Failable> Serialize(T target, Stream output, CancellationToken cancellation = default);
}