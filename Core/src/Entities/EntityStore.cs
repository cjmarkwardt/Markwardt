namespace Markwardt;

public interface IEntityStore : IEntityLoader, IMultiDisposable
{
    [Factory<EntityStore>]
    delegate ValueTask<IEntityStore> Factory(IDataStore store, IEntityExpirationCalculator? expirationCalculator = null);

    IEntityClaim Create(string? id = null);
    ValueTask Save();
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

        this.handler.Add(new EntityIdDataGenerator());

        this.LoopInBackground(TimeSpan.FromSeconds(1), async _ => await Clean());
    }

    private readonly IDataSegmentTyper segmentTyper;
    private readonly IDataHandler handler;
    private readonly IEntityIdCreator idCreator;
    private readonly IDataStore store;
    private readonly IEntityExpirationCalculator expirationCalculator;

    private readonly SequentialExecutor saveExecutor = new();
    private readonly Dictionary<string, IEntityHandle> handles = [];

    public IEntityClaim Create(string? id = null)
    {
        (EntityHandle handle, EntityClaim claim) = EntityHandle.NewEntity(new DataEntity(idCreator.Create(id), segmentTyper, handler, []), expirationCalculator);
        handles.Add(claim.Id, handle);
        return claim;
    }

    public async ValueTask<IEnumerable<IEntityClaim>> Load(IEnumerable<string> ids)
    {
        IReadOnlyDictionary<string, TaskCompletionSource<IDataEntity>> loadCompletions = ids.Where(x => !handles.ContainsKey(x)).ToDictionary(x => x, _ => new TaskCompletionSource<IDataEntity>());
        IReadOnlyList<IEntityHandle> targetHandles = ids.Select(x => GetHandle(x, async () => await loadCompletions[x].Task)).ToList();

        foreach (KeyValuePair<string, DataDictionary> loadedEntity in await store.Load(loadCompletions.Keys))
        {
            loadCompletions[loadedEntity.Key].SetResult(new DataEntity(loadedEntity.Key, segmentTyper, handler, loadedEntity.Value));
        }

        return await Task.WhenAll(targetHandles.Select(x => x.Claim().AsTask()));
    }

    public async ValueTask Save()
        => await saveExecutor.Execute(async () => await Save(handles.Values));

    private IEntityHandle GetHandle(string id, Func<Task<IDataEntity>> load)
    {
        if (!handles.TryGetValue(id, out IEntityHandle? handle))
        {
            handle = new EntityHandle(load(), expirationCalculator);
            handles.Add(id, handle);
        }

        return handle;
    }

    private async ValueTask Clean()
    {
        if (handles.Any(x => x.Value.IsExpired))
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
        => await store.Save(handles.SelectMaybe(x => x.PopChanged()).Select(x => new KeyValuePair<string, DataDictionary>(x.Id, x.Data)));
}