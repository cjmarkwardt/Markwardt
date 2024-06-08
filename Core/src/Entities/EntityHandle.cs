namespace Markwardt;

public interface IEntityHandle
{
    bool IsExpired { get; }

    ValueTask<IDisposable<IEntity>> Claim();
    Maybe<IDataEntity> PopChanged();
}

public sealed class EntityHandle(Task<IDataEntity> load, IEntityExpirationCalculator expirationCalculator) : IEntityHandle
{
    public static (EntityHandle Handle, IDisposable<IEntity> Entity) NewEntity(IDataEntity entity, IEntityExpirationCalculator expirationCalculator)
    {
        EntityHandle handle = new(Task.FromResult(entity), expirationCalculator);
        return (handle, handle.CreateClaim(entity));
    }

    private int claims;
    private DateTime? expiration;

    public bool IsExpired => expiration is not null && DateTime.Now > expiration;

    public async ValueTask<IDisposable<IEntity>> Claim()
        => CreateClaim(await load);

    public Maybe<IDataEntity> PopChanged()
    {
        if (!load.IsCompleted || !load.Result.Data.PopChanges())
        {
            return default;
        }
        else
        {
            return load.Result.Maybe();
        }
    }

    private IDisposable<IEntity> CreateClaim(IEntity content)
    {   
        claims++;
        expiration = null;
        return new Disposable<IEntity>(content, [Disposable.Create(DestroyClaim)]);
    }

    private void DestroyClaim()
    {
        claims--;

        if (claims == 0)
        {
            expiration = DateTime.Now + expirationCalculator.Calculate(load.Result);
        }
    }
}