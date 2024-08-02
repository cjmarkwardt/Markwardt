namespace Markwardt;

public interface IEntityExpirationCalculator
{
    TimeSpan Calculate(IEntityOld entity);
}

public class EntityExpirationCalculator(TimeSpan expiration) : IEntityExpirationCalculator
{
    public TimeSpan Calculate(IEntityOld entity)
        => expiration;
}