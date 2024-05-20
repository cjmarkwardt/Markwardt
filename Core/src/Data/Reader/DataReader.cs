namespace Markwardt;

public class DataReader<T>(IDataHandler handler)
{
    public T Read(IDataNode node)
        => (T)handler.Deserialize(typeof(T), node.AsValue()?.Content).NotNull();

    public IDataNode Write(T item)
        => new DataValue(handler.Serialize(typeof(T), item).NotNull());
}