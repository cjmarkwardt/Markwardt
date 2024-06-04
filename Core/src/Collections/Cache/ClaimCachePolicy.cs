namespace Markwardt;

public interface IClaimCachePolicy<T>
{
    bool IsExpired(T item, int claims, DateTime lastClaim, DateTime? lastRelease);
}

public record ClaimCachePolicy<T>(Func<T, int, DateTime, DateTime?, bool> isExpired) : IClaimCachePolicy<T>
{
    public bool IsExpired(T item, int claims, DateTime lastClaim, DateTime? lastRelease)
        => isExpired(item, claims, lastClaim, lastRelease);
}