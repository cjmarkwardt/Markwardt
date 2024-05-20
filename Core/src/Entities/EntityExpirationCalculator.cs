namespace Markwardt;

public interface IEntityExpirationCalculator
{
    TimeSpan Calculate(IEntity entity);
}

public class EntityExpirationCalculator(TimeSpan expiration) : IEntityExpirationCalculator
{
    public TimeSpan Calculate(IEntity entity)
        => expiration;
}