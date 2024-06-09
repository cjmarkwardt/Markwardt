namespace Markwardt;

public interface IClaimCachePolicy<in TKey, in T>
{
    bool IsExpired(TKey key, T item, int claims, DateTime lastClaim, DateTime? lastRelease);
}

public class ClaimCachePolicy<TKey, T>(ClaimCachePolicy<TKey, T>.Delegate @delegate) : IClaimCachePolicy<TKey, T>
{
    public delegate bool Delegate(TKey key, T item, int claims, DateTime lastClaim, DateTime? lastRelease);

    public bool IsExpired(TKey key, T item, int claims, DateTime lastClaim, DateTime? lastRelease)
        => @delegate(key, item, claims, lastClaim, lastRelease);
}