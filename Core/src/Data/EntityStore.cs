namespace Markwardt;

public interface IEntityStore
{
    ValueTask<IEntity> Create();
    ValueTask<IEntity> Load(int id);
    ValueTask Save(IEntity entity);
    ValueTask Delete(int id);

    async ValueTask Delete(IEntity entity)
        => await Delete(entity.Id);
}

public class EntityStore(IDataStore store) : IEntityStore
{
    public async ValueTask<IEntity> Create()
    {
        int id = await store.Create();

    }

    public ValueTask<IEntity> Load(int id)
    {
        throw new NotImplementedException();
    }

    public ValueTask Save(IEntity entity)
    {
        throw new NotImplementedException();
    }

    public ValueTask Delete(int id)
    {
        throw new NotImplementedException();
    }

    private sealed class Entity(int id, IDataStore store) : IEntity
    {
        private readonly Dictionary<Type, object> sections = [];

        public int Id => id;

        public bool Contains(Type type)
            => sections.ContainsKey(type);

        public object Add(Type type)
        {
            object section = Activator.CreateInstance(type).NotNull();
            sections.Add(type, section);
            return section;
        }

        public object Get(Type type)
            => sections[type];

        public void Delete(Type type)
            => sections.Remove(type);
    }
}