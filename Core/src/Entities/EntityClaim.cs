namespace Markwardt;

public interface IEntityClaim : IEntityOld, IDisposable;

public sealed class EntityClaim(IEntityOld entity, Action dispose) : IEntityClaim
{
    private bool isDisposed;

    public EntityId Id => entity.Id;

    public IEnumerable<string> Flags => GetEntity().Flags;
    public IEnumerable<string> Sections => GetEntity().Sections;

    public bool HasFlag(string flag)
        => GetEntity().HasFlag(flag);

    public void SetFlag(string flag)
        => GetEntity().SetFlag(flag);

    public void ClearFlag(string flag)
        => GetEntity().ClearFlag(flag);

    public bool ContainsSection<T>()
        where T : class
        => GetEntity().ContainsSection<T>();

    public void DeleteSection<T>()
        where T : class
        => GetEntity().DeleteSection<T>();

    public T GetSection<T>()
        where T : class
        => GetEntity().GetSection<T>();

    public bool Equals(IEntityOld? other) => entity.Equals(other);
    public override bool Equals(object? obj) => obj is IEntityOld other && Equals(other);
    public override int GetHashCode() => Id.GetHashCode();

    public void Dispose()
    {
        if (!isDisposed)
        {
            isDisposed = true;
            dispose();
        }
    }

    private IEntityOld GetEntity()
        => !isDisposed ? entity : throw new ObjectDisposedException(GetType().Name);
}