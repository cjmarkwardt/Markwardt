namespace Markwardt;

public abstract record MatchFilter
{
    public record PropertyTextEqual(string Key, string Value) : MatchFilter;
    public record PropertyTextNotEqual(string Key, string Value) : MatchFilter;

    public record PropertyEqual(string Key, int Value) : MatchFilter;
    public record PropertyNotEqual(string Key, int Value) : MatchFilter;
    public record PropertyAtMost(string Key, int Value) : MatchFilter;
    public record PropertyAtLeast(string Key, int Value) : MatchFilter;

    public record ResultsAtMost(int Value) : MatchFilter;
    public record DistanceAtMost(float Value) : MatchFilter;
}