namespace Markwardt;

[Transient<TimeoutAssetPolicy>]
public interface IAssetPolicy
{
    bool IsExpired(string key, object value, DateTime lastClaim, DateTime? lastRelease);
}