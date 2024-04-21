namespace Markwardt;

public class SteamMatchSource : IMatchSource
{
    public required ISteamInitializer Initializer { get; init; }

    public async ValueTask<Failable<IEnumerable<IMatch>>> GetMatches(IEnumerable<MatchFilter> filters, CancellationToken cancellation = default)
    {
        Failable tryInitialize = Initializer.Initialize();
        if (tryInitialize.Exception is not null)
        {
            return tryInitialize.Exception.AsFailable<IEnumerable<IMatch>>();
        }

        foreach (MatchFilter filter in filters)
        {
            if (filter is MatchFilter.PropertyTextEqual propertyTextEqual)
            {
                SteamMatchmaking.AddRequestLobbyListStringFilter(propertyTextEqual.Key, propertyTextEqual.Value, ELobbyComparison.k_ELobbyComparisonEqual);
            }
            else if (filter is MatchFilter.PropertyTextNotEqual propertyTextNotEqual)
            {
                SteamMatchmaking.AddRequestLobbyListStringFilter(propertyTextNotEqual.Key, propertyTextNotEqual.Value, ELobbyComparison.k_ELobbyComparisonNotEqual);
            }
            else if (filter is MatchFilter.PropertyEqual propertyEqual)
            {
                SteamMatchmaking.AddRequestLobbyListNumericalFilter(propertyEqual.Key, propertyEqual.Value, ELobbyComparison.k_ELobbyComparisonEqual);
            }
            else if (filter is MatchFilter.PropertyNotEqual propertyNotEqual)
            {
                SteamMatchmaking.AddRequestLobbyListNumericalFilter(propertyNotEqual.Key, propertyNotEqual.Value, ELobbyComparison.k_ELobbyComparisonNotEqual);
            }
            else if (filter is MatchFilter.PropertyAtMost propertyAtMost)
            {
                SteamMatchmaking.AddRequestLobbyListNumericalFilter(propertyAtMost.Key, propertyAtMost.Value, ELobbyComparison.k_ELobbyComparisonEqualToOrLessThan);
            }
            else if (filter is MatchFilter.PropertyAtLeast propertyAtLeast)
            {
                SteamMatchmaking.AddRequestLobbyListNumericalFilter(propertyAtLeast.Key, propertyAtLeast.Value, ELobbyComparison.k_ELobbyComparisonEqualToOrGreaterThan);
            }
            else if (filter is MatchFilter.ResultsAtMost resultsAtMost)
            {
                SteamMatchmaking.AddRequestLobbyListResultCountFilter(resultsAtMost.Value);
            }
            else if (filter is MatchFilter.DistanceAtMost distanceAtMost)
            {
                SteamMatchmaking.AddRequestLobbyListDistanceFilter(distanceAtMost.Value switch
                {
                    < 0.33f => ELobbyDistanceFilter.k_ELobbyDistanceFilterClose,
                    < 0.66f => ELobbyDistanceFilter.k_ELobbyDistanceFilterFar,
                    _ => ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide
                });
            }
        }

        Failable<LobbyMatchList_t> tryRequestLobbies = await SteamMatchmaking.RequestLobbyList().Consume<LobbyMatchList_t>(cancellation);
        if (tryRequestLobbies.Exception is not null)
        {
            return tryRequestLobbies.Exception;
        }

        return Enumerable.Range(0, (int)tryRequestLobbies.Result.m_nLobbiesMatching).Select(x => new SteamMatch(SteamMatchmaking.GetLobbyByIndex(x))).AsFailable<IEnumerable<IMatch>>();
    }
}