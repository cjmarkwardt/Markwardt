namespace Markwardt;

public interface IEntityClaim : IEntity, IDisposable;

public sealed class EntityClaim(IEntity entity, Action dispose) : IEntityClaim
{
    private bool isDisposed;

    public string Id => entity.Id;

    public IEnumerable<string> Sections => GetEntity().Sections;

    public bool ContainsSection<T>()
        where T : class
        => GetEntity().ContainsSection<T>();

    public void DeleteSection<T>()
        where T : class
        => GetEntity().DeleteSection<T>();

    public T GetSection<T>()
        where T : class
        => GetEntity().GetSection<T>();

    public bool Equals(IEntity? other) => entity.Equals(other);
    public override bool Equals(object? obj) => obj is IEntity other && Equals(other);
    public override int GetHashCode() => Id.GetHashCode();

    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            dispose();
        }
    }

    private IEntity GetEntity()
        => !isDisposed ? entity : throw new ObjectDisposedException(GetType().Name);
}