namespace Markwardt;

public class TypeArgumentComparer : IEqualityComparer<IReadOnlyDictionary<string, Type>>
{
    private readonly PairComparer pairComparer = new();

    public bool Equals(IReadOnlyDictionary<string, Type>? x, IReadOnlyDictionary<string, Type>? y)
        => ReferenceEquals(x, y) || (x != null && y != null && x.OrderBy(z => z.Key).SequenceEqual(y.OrderBy(z => z.Key), pairComparer));

    public int GetHashCode([DisallowNull] IReadOnlyDictionary<string, Type> obj)
    {
        HashCode hash = new();
        foreach (KeyValuePair<string, Type> pair in obj)
        {
            hash.Add(pair.Key);
            hash.Add(pair.Value);
        }

        return hash.ToHashCode();
    }

    private sealed class PairComparer : IEqualityComparer<KeyValuePair<string, Type>>
    {
        public bool Equals(KeyValuePair<string, Type> x, KeyValuePair<string, Type> y)
            => x.Key == y.Key && x.Value == y.Value;

        public int GetHashCode(KeyValuePair<string, Type> obj)
            => HashCode.Combine(obj.Key, obj.Value);
    }
}