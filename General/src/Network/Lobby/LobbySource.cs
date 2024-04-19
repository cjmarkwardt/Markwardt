namespace Markwardt;

public interface ILobbySource
{
    ValueTask<IEnumerable<ILobbyProfile>> GetLobbies(IEnumerable<LobbyFilter> filters, CancellationToken cancellation = default);
}

public static class LobbySourceExtensions
{
    public static async ValueTask<IEnumerable<ILobbyProfile>> GetLobbies(ILobbySource source, CancellationToken cancellation = default)
        => await source.GetLobbies([], cancellation);
}