namespace Markwardt;

public interface IEntityHandle
{
    bool IsExpired { get; }

    ValueTask<IEntityClaim> Claim();
    Maybe<IDataEntity> PopChanged();
}

public sealed class EntityHandle(Task<IDataEntity> load, IEntityExpirationCalculator expirationCalculator) : IEntityHandle
{
    public static (EntityHandle Handle, EntityClaim Claim) NewEntity(IDataEntity entity, IEntityExpirationCalculator expirationCalculator)
    {
        EntityHandle handle = new(Task.FromResult(entity), expirationCalculator);
        return (handle, handle.CreateClaim(entity));
    }

    private int claims;
    private DateTime? expiration;

    public bool IsExpired => expiration is not null && DateTime.Now > expiration;

    public async ValueTask<IEntityClaim> Claim()
        => CreateClaim(await load);

    public Maybe<IDataEntity> PopChanged()
    {
        if (!load.IsCompleted)
        {
            return default;
        }
        else
        {
            return load.Result.Maybe();
        }
    }

    private EntityClaim CreateClaim(IEntityOld content)
    {   
        claims++;
        expiration = null;
        return new EntityClaim(content, DestroyClaim);
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