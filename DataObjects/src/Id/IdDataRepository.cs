namespace Markwardt;

public interface IIdDataRepository : IDataRepository, IIdDataLoader, IComplexDisposable
{
    [Factory<IdDataRepository>]
    delegate IIdDataRepository Factory(IIdDataSource source);
}

public class IdDataRepository : ComplexDisposable, IIdDataRepository, IIdDataNotifier
{
    public IdDataRepository(IIdDataSource depot)
    {
        this.depot = depot.DisposeWith(this);
        directLoader = new(this);
        executor.DisposeWith(this);
    }

    private readonly IIdDataSource depot;
    private readonly DirectLoader directLoader;
    private readonly SequentialExecutor executor = new();
    private readonly TypeNamer typeNamer = new();
    private readonly IdGenerator idGenerator = new();
    private readonly DataCache<string, IdDataModel> entities = new(x => x.Id);
    private readonly HashSet<string> expiredEntities = [];

    private HashSet<string> deletedEntities = [];
    private Dictionary<string, string> changedIndexes = [];
    private HashSet<string> deletedIndexes = [];

    public async ValueTask<IDataObject?> TryFindIndex(string index)
        => await executor.Execute(async () =>
        {
            string? id = await depot.TryLoadIndex(index);
            if (id == null)
            {
                return null;
            }

            return await DirectLoad(id);
        });

    public async ValueTask<IDataObject?> TryLoad(string id)
        => await executor.Execute(async () => await DirectLoad(id));

    public void DeleteIndex(string index)
    {
        changedIndexes.Remove(index);
        deletedIndexes.Add(index);
    }

    public async ValueTask Save()
        => await executor.Execute(async () =>
        {
            IEnumerable<IdData> previousChangedEntities = entities.Select(x => x.Export()).ToList();
            entities.Clear();
            expiredEntities.Clear();

            IEnumerable<string> previousDeletedEntities = deletedEntities;
            deletedEntities = [];

            IEnumerable<KeyValuePair<string, string>> previousChangedIndexes = changedIndexes;
            changedIndexes = [];

            IEnumerable<string> previousDeletedIndexes = deletedIndexes;
            deletedIndexes = [];

            await depot.Save(previousChangedEntities, previousDeletedEntities, previousChangedIndexes, previousDeletedIndexes);
        });

    public async ValueTask Clean()
        => await executor.Execute(async () =>
        {
            IEnumerable<IdData> changedEntities = expiredEntities.Select(x => entities.PopKey(x).Export()).ToList();
            expiredEntities.Clear();
            await depot.Save(changedEntities, Enumerable.Empty<string>(), Enumerable.Empty<KeyValuePair<string, string>>(), Enumerable.Empty<string>());
        });

    void IIdDataNotifier.MarkEntityDeleted(string id)
        => deletedEntities.Add(id);

    void IIdDataNotifier.MarkEntityExpired(string id)
    {
        if (entities.ContainsKey(id))
        {
            expiredEntities.Add(id);
        }
    }

    void IIdDataNotifier.MarkIndexChanged(string index, string entityId)
    {
        changedIndexes[index] = entityId;
        deletedIndexes.Remove(index);
    }

    protected void Register<TEntity>(string? name = null)
        where TEntity : class, IDataObject
        => typeNamer.Register(typeof(TEntity), name);

    protected async ValueTask<TEntity> Create<TEntity>(params (string Name, object? Value)[] arguments)
        where TEntity : class, IDataObject
        => (TEntity)Create(await IdDataModel.Create(this, this, directLoader, typeNamer, idGenerator.Generate(), typeof(TEntity), arguments.ToDictionary(x => x.Name, x => x.Value)));

    private IDataObject Create(IdDataModel model)
    {
        entities.Add(model);
        return model.CreateClaim();
    }

    private async ValueTask<IDataObject?> DirectLoad(string id)
    {
        expiredEntities.Remove(id);

        if (entities.TryGetValue(id, out IdDataModel? entity))
        {
            return entity.CreateClaim();
        }

        IdData? data = await depot.TryLoadEntity(id);
        if (data == null)
        {
            return null;
        }

        return Create(await IdDataModel.Load(this, this, directLoader, typeNamer, data));
    }

    private sealed class DirectLoader(IdDataRepository repository) : IIdDataLoader
    {
        public async ValueTask<IDataObject?> TryLoad(string id)
            => await repository.DirectLoad(id);
    }
}