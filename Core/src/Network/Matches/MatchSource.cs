namespace Markwardt;

public interface IMatchSource
{
    ValueTask<Failable<IEnumerable<IMatch>>> GetMatches(IEnumerable<MatchFilter> filters, CancellationToken cancellation = default);
}

public static class MatchSourceExtensions
{
    public static async ValueTask<Failable<IEnumerable<IMatch>>> GetMatches(this IMatchSource source, CancellationToken cancellation = default)
        => await source.GetMatches([], cancellation);
}