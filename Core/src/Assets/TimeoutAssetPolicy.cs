namespace Markwardt;

public class TimeoutAssetPolicy : IAssetPolicy
{
    public bool IsExpired(string key, object value, DateTime lastClaim, DateTime? lastRelease)
        => DateTime.Now > lastRelease?.Add(GetTimeout(key, value));

    protected virtual TimeSpan GetTimeout(string key, object value)
        => TimeSpan.FromMinutes(5);
}