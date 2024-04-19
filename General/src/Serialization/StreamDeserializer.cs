namespace Markwardt;

public interface IStreamDeserializer<T>
{
    ValueTask<Failable<T>> Deserialize(Stream input, CancellationToken cancellation = default);
}