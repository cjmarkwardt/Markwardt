namespace Markwardt;

public interface IEntity : IEquatable<IEntity>
{
    string Id { get; }
    IEnumerable<string> Sections { get; }

    bool ContainsSection<T>()
        where T : class;

    void DeleteSection<T>()
        where T : class;

    T GetSection<T>()
        where T : class;
}