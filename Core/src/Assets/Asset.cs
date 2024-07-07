namespace Markwardt;

public interface IAsset
{
    string Key { get; }
    object Value { get; }
    bool IsExpired { get; }

    bool ForceExpire { get; set; }

    IDisposable<object> Claim();
}