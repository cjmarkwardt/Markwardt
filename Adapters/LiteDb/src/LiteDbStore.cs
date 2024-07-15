namespace Markwardt;

/*public delegate ValueTask<LiteDbStore> LiteDbStoreFactory(string path);

public class LiteDbStore : ExtendedDisposable, IDataStore
{
    public LiteDbStore(string path)
        => entries = new LiteDatabase(path).DisposeWith(this).GetCollection<BsonDocument>("Entries");

    private readonly ILiteCollection<BsonDocument> entries;

    public async ValueTask Save(IEnumerable<KeyValuePair<string, DataDictionary>> entries)
        => await Task.Run(() => this.entries.Upsert(entries.Select(x => new BsonDocument() { { "_id", x.Key }, { "Content", Serialize(x.Value) } })));

    public async ValueTask<IReadOnlyDictionary<string, DataDictionary>> Load(IEnumerable<string> ids)
    {
        HashSet<string> idLookup = ids.ToHashSet();
        return await Task.Run(() => entries.Find(x => idLookup.Contains(x)).ToDictionary(x => x["_id"].AsString, x => (DataDictionary)Deserialize(x["Content"])));
    }

    private BsonValue Serialize(IDataNode input)
    {
        if (input is DataValue value)
        {
            return new BsonValue(value.Content);
        }
        else if (input is DataList list)
        {
            return new BsonArray(list.Select(Serialize));
        }
        else if (input is DataDictionary dictionary)
        {
            return new(dictionary.ToDictionary(x => x.Key, x => Serialize(x.Value))) { ["~type"] = dictionary.Type };
        }
        else
        {
            throw new NotSupportedException(input.GetType().Name);
        }
    }

    private IDataNode Deserialize(BsonValue input)
    {
        if (input is BsonArray array)
        {
            return new DataList(array.Select(Deserialize));
        }
        else if (input is BsonDocument document)
        {
            return new DataDictionary(document.Select(x => new KeyValuePair<string, IDataNode>(x.Key, Deserialize(x.Value)))) { Type = document["~type"] };
        }
        else if (input is BsonValue value)
        {
            return new DataValue(value.AsString);
        }
        else
        {
            throw new NotSupportedException(input.GetType().Name);
        }
    }
}*/