namespace Markwardt;

public class DataKeyReader<T>(IDataHandler handler)
{
    private readonly DataReader<T> writer = new(handler);

    public T Read(string text)
        => writer.Read(new DataValue(text));

    public string Write(T item)
        => writer.Write(item)?.AsValue()?.Content ?? throw new InvalidOperationException();
}