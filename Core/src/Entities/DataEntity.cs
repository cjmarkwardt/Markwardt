namespace Markwardt;

public interface IDataEntity : IEntityOld
{
    //DataDictionary Data { get; }
}

/*public sealed class DataEntity(string id, IDataSegmentTyper segmentTyper, IDataHandler handler, DataDictionary data) : IDataEntity
{
    private readonly IModel model = data.AsSegment<IModel>(segmentTyper, handler);

    public EntityId Id => new(id);
    public DataDictionary Data => data;

    public IEnumerable<string> Flags => model.Flags;
    public IEnumerable<string> Sections => data.Keys;

    public bool HasFlag(string flag)
        => model.Flags.Contains(flag);

    public void SetFlag(string flag)
        => model.Flags.Add(flag);

    public void ClearFlag(string flag)
        => model.Flags.Remove(flag);

    public bool ContainsSection<T>()
        where T : class
        => model.Sections.ContainsKey(segmentTyper.GetName(typeof(T)));

    public void DeleteSection<T>()
        where T : class
        => model.Sections.Remove(segmentTyper.GetName(typeof(T)));
        
    public T GetSection<T>()
        where T : class
        => model.Sections.GetOrAdd<T>(segmentTyper.GetName(typeof(T)));

    public bool Equals(IEntity? other)
        => other is not null && Id == other.Id;

    public override bool Equals(object? obj)
        => obj is IEntity other && Equals(other);

    public override int GetHashCode()
        => Id.GetHashCode();

    public interface IModel
    {
        ISet<string> Flags { get; }
        ISegmentDictionary<string, object> Sections { get; }
    }
}*/