namespace Markwardt;

public interface IEntity : IEquatable<IEntity>
{
    string Id { get; }

    IEnumerable<string> Flags { get; }
    IEnumerable<string> Sections { get; }

    bool HasFlag(string flag);

    void SetFlag(string flag);

    void ClearFlag(string flag);

    bool ContainsSection<T>()
        where T : class;

    void DeleteSection<T>()
        where T : class;

    T GetSection<T>()
        where T : class;
}