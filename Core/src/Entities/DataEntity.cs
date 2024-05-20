namespace Markwardt;

public interface IDataEntity : IEntity
{
    DataDictionary Data { get; }
}

public sealed class DataEntity(string id, IDataSegmentTyper segmentTyper, IDataHandler handler, DataDictionary data) : IDataEntity
{
    public string Id => id;
    public DataDictionary Data => data;

    public IEnumerable<string> Sections => data.Keys;

    public bool ContainsSection<T>()
        where T : class
        => data.ContainsKey(GetSectionName<T>());

    public void DeleteSection<T>()
        where T : class
        => data.Remove(GetSectionName<T>());
        
    public T GetSection<T>()
        where T : class
    {
        DataDictionary dictionary;
        if (ContainsSection<T>())
        {
            dictionary = data[GetSectionName<T>()].AsDictionary() ?? throw new InvalidOperationException();
        }
        else
        {
            dictionary = [];
            data.Add(GetSectionName<T>(), dictionary);
        }

        return DataSegment.Adapt<T>(segmentTyper, handler, dictionary);
    }

    public bool Equals(IEntity? other)
        => other is not null && Id == other.Id;

    public override bool Equals(object? obj)
        => obj is IEntity other && Equals(other);

    public override int GetHashCode()
        => Id.GetHashCode();

    private string GetSectionName<T>()
        => typeof(T).GetCustomAttribute<SegmentAttribute>()?.Name ?? throw new InvalidOperationException($"Type {typeof(T)} is not an entity section");
}