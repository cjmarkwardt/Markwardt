namespace Markwardt;

public interface IEntityStore : IEntityLoader, IMultiDisposable
{
    [Factory<EntityStore>]
    delegate ValueTask<IEntityStore> Factory(IDataStore store, IEntityExpirationCalculator? expirationCalculator = null);

    EntityId GetId(string id);
    ValueTask<IDisposable<IEntity>> Create(string? id = null);
    ValueTask Save();
}

public static class EntityStoreExtensions
{
    public static async ValueTask<IDisposable<IEntity>> Load(this IEntityStore store, string id)
        => await store.Load(store.GetId(id));
}

public class EntityStore : ExtendedDisposable, IEntityStore
{
    public EntityStore(IDataSegmentTyper segmentTyper, IDataHandler handler, IEntityIdCreator idCreator, IDataStore store, IEntityExpirationCalculator? expirationCalculator = null)
    {
        this.segmentTyper = segmentTyper;
        this.handler = handler;
        this.idCreator = idCreator;
        this.store = store.DisposeWith(this);
        this.expirationCalculator = expirationCalculator ?? new EntityExpirationCalculator(TimeSpan.FromMinutes(5));

        async ValueTask<IDictionary<EntityId, DataEntity>> Loader(IEnumerable<EntityId> keys)
            => (await store.Load(keys.Select(x => x.Value))).Select(x => new DataEntity(x.Key, segmentTyper, handler, x.Value)).ToDictionary(x => x.Id);

        entities = new(new ClaimCachePolicy<DataEntity>((item, claims, lastClaim, lastRelease) => claims == 0 && lastClaim.AddMinutes(5) > DateTime.Now), Loader);
        entities.Removed.Subscribe(Clean);

        this.LoopInBackground(TimeSpan.FromSeconds(1), async _ => await Clean());
    }

    private readonly IDataSegmentTyper segmentTyper;
    private readonly IDataHandler handler;
    private readonly IEntityIdCreator idCreator;
    private readonly IDataStore store;
    private readonly IEntityExpirationCalculator expirationCalculator;

    private readonly SequentialExecutor saveExecutor = new();
    private readonly Dictionary<string, IEntityHandle> handles = [];
    private readonly AsyncClaimCache<EntityId, DataEntity> entities;

    public EntityId GetId(string id)
        => new(idCreator.Create(id));

    public async ValueTask<IDisposable<IEntity>> Create(string? id = null)
    {
        DataEntity entity = new(idCreator.Create(id), segmentTyper, handler, []);
        return (await entities.Claim(entity.Id)).Value;
    }

    public async ValueTask<IEnumerable<IDisposable<IEntity>>> Load(IEnumerable<EntityId> ids)
    {


        IReadOnlyDictionary<string, TaskCompletionSource<IDataEntity>> loadCompletions = ids.Where(x => !entities.Contains(x.Value)).ToDictionary(x => x.Value, _ => new TaskCompletionSource<IDataEntity>());
        IReadOnlyList<IEntityHandle> targetHandles = ids.Select(x => GetHandle(x.Value, async () => await loadCompletions[x.Value].Task)).ToList();

        foreach (KeyValuePair<string, DataDictionary> loadedEntity in await store.Load(loadCompletions.Keys))
        {
            loadCompletions[loadedEntity.Key].SetResult(new DataEntity(loadedEntity.Key, segmentTyper, handler, loadedEntity.Value));
        }

        return await Task.WhenAll(targetHandles.Select(x => x.Claim().AsTask()));
    }

    public async ValueTask Save()
        => await saveExecutor.Execute(async () => await Save(cache));

    private IEntityHandle GetHandle(string id, Func<Task<IDataEntity>> load)
    {
        if (!handles.TryGetValue(id, out IEntityHandle? handle))
        {
            handle = new EntityHandle(load(), expirationCalculator);
            handles.Add(id, handle);
        }

        return handle;
    }

    private async ValueTask Clean(IEnumerable<DataEntity> entities)
    {
        if (entities.Any(x => x.Value.IsExpired))
        {
            await saveExecutor.Execute(async () =>
            {
                IReadOnlyList<KeyValuePair<string, IEntityHandle>> expiredHandles = handles.Where(x => x.Value.IsExpired).ToList();
                await Save(expiredHandles.Select(x => x.Value));

                foreach (KeyValuePair<string, IEntityHandle> expiredHandle in expiredHandles.Where(x => x.Value.IsExpired))
                {
                    handles.Remove(expiredHandle.Key);
                }
            });
        }
    }

    private async ValueTask Save(IEnumerable<IEntityHandle> handles)
        => await store.Save(handles.SelectMaybe(x => x.PopChanged()).Select(x => new KeyValuePair<string, DataDictionary>(x.Id.Value, x.Data)));
}