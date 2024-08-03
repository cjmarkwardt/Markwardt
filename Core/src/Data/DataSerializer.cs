namespace Markwardt;

public interface IDataHandler
{
    void Handle(Type? type, Func<ITypeDescription, bool> isHandled, IDataSubSerializer serializer);
}

public interface IDataSerializer
{
    void Serialize(ITypeDescription target, IDataWriter writer, object? value);
    object? Deserialize(ITypeDescription target, IDataReader reader);
}

public class DataSerializer : IDataSerializer, IDataHandler
{
    private readonly IDataSubSerializer fallback = new AutoDataSerializer();
    private readonly Dictionary<Type, Handler> keyedHandlers = [];
    private readonly List<Handler> handlers = [];

    public void Handle(Type? type, Func<ITypeDescription, bool> isHandled, IDataSubSerializer serializer)
    {
        Handler handler = new(isHandled, serializer);
        handlers.Add(handler);

        if (type is not null)
        {
            keyedHandlers[Nullable.GetUnderlyingType(type) ?? type] = new(isHandled, serializer);
        }
    }

    public void Serialize(ITypeDescription target, IDataWriter writer, object? value)
    {
        if (value is null)
        {
            writer.WriteNull();
        }
        else
        {
            GetSerializer(target).Serialize(target, writer, value, this);
        }
    }

    public object? Deserialize(ITypeDescription target, IDataReader reader)
    {
        object? value = reader.Read();
        if (value is null)
        {
            if (target.IsNullable)
            {
                throw new InvalidOperationException();
            }

            return null;
        }

        return GetSerializer(target).Deserialize(target, reader, this);
    }

    private IDataSubSerializer GetSerializer(ITypeDescription target)
    {
        if (keyedHandlers.TryGetValue(Nullable.GetUnderlyingType(target.Type) ?? target.Type, out Handler? handler) && handler.IsHandled(target))
        {
            return handler.Serializer;
        }
        else
        {
            return handlers.Find(x => x.IsHandled(target))?.Serializer ?? fallback;
        }
    }

    private sealed record Handler(Func<ITypeDescription, bool> IsHandled, IDataSubSerializer Serializer);
}