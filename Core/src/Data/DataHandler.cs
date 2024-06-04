namespace Markwardt;

[Transient<DataHandler>]
public interface IDataHandler
{
    void Add(IDataGenerator generator);

    object? Deserialize(Type type, string? data);
    string? Serialize(Type type, object? value);
}

public class DataHandler : IDataHandler
{
    private readonly Dictionary<Type, IDataSerializer> serializers = [];

    private readonly LinkedList<IDataGenerator> generators = new
    ([
        new StringDataGenerator(),
        new IntDataGenerator(),
        new FloatDataGenerator(),
        new BoolDataGenerator(),
        new EnumDataGenerator(),
        new EntityIdDataGenerator(),
        new AssetIdDataGenerator(),
        new DateTimeDataGenerator(),
        new TimeSpanDataGenerator(),
        new Vector2DataGenerator(),
        new Vector3DataGenerator(),
        new Vector4DataGenerator(),
        new ColorDataGenerator(),
        new LongDataGenerator(),
        new DoubleDataGenerator(),
        new DecimalDataGenerator(),
        new ByteDataGenerator(),
    ]);

    public void Add(IDataGenerator generator)
        => generators.AddFirst(generator);

    public object? Deserialize(Type type, string? data)
        => GetSerializer(type).Deserialize(data);

    public string? Serialize(Type type, object? value)
        => GetSerializer(type).Serialize(value);

    private IDataSerializer GetSerializer(Type type)
    {
        if (!serializers.TryGetValue(type, out IDataSerializer? serializer))
        {
            foreach (IDataGenerator generator in generators)
            {
                serializer = generator.Generate(type);
                if (serializer is not null)
                {
                    return serializer;
                }
            }

            throw new InvalidOperationException($"No data serializer available for type {type}");
        }

        return serializer;
    }
}