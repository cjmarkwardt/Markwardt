namespace Markwardt;

public class SteamMessageHandle(nint value) : Handle<nint>(value, x => SteamNetworkingMessage_t.Release(x))
{
    public Lazy<ReadOnlyMemory<byte>> Data { get; } = new(() =>
    {
        SteamNetworkingMessage_t message = SteamNetworkingMessage_t.FromIntPtr(value);
        return message.m_pData.ToArray(message.m_cbSize);
    });
}