namespace Markwardt;

public class LiteDbSource : IIdDataSource
{
    public LiteDbSource(LiteDatabase database)
    {
        this.database = database;
        entities = database.GetCollection<IdData>("Entities");
        indexes = database.GetCollection("Indexes");
    }

    public LiteDbSource(string path)
        : this(new LiteDatabase(path)) { }

    private readonly LiteDatabase database;
    private readonly ILiteCollection<IdData> entities;
    private readonly ILiteCollection<BsonDocument> indexes;

    public async ValueTask<IdData?> TryLoadEntity(string id)
        => await Task.Run(() => entities.FindById(id) ?? throw new InvalidOperationException($"Entity {id} not found"));

    public async ValueTask<string?> TryLoadIndex(string index)
        => await Task.Run(() => indexes.FindById(index)?["EntityId"]);

    public async ValueTask Save(IEnumerable<IdData> entities, IEnumerable<string> deletedEntities, IEnumerable<KeyValuePair<string, string>> indexes, IEnumerable<string> deletedIndexes)
        => await Task.Run(() =>
        {
            try
            {
                database.BeginTrans();

                this.entities.Upsert(entities);

                foreach (string id in deletedEntities)
                {
                    this.entities.Delete(id);
                }

                this.indexes.Upsert(indexes.Select(x => new BsonDocument() { ["_id"] = x.Key, ["EntityId"] = x.Value }));

                foreach (string index in deletedIndexes)
                {
                    this.indexes.Delete(index);
                }

                database.Commit();
            }
            catch
            {
                database.Rollback();
                throw;
            }
        });

    public void Dispose()
        => database.Dispose();

    public ValueTask DisposeAsync()
    {
        database.Dispose();
        return default;
    }
}