namespace Markwardt;

public interface IDeserializer<T>
{
    ValueTask<Failable<T>> Deserialize(Stream input, CancellationToken cancellation = default);
}