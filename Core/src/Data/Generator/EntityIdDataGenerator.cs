namespace Markwardt;

public class EntityIdDataGenerator : DataGenerator<EntityId, EntityIdDataGenerator.Serializer>
{
    public class Serializer : DataSerializer<EntityId?>
    {
        protected override EntityId? Read(string data)
            => new(data);
    }
}