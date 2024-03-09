namespace Markwardt;

public static class RangeExtensions
{
    public static IEnumerable<T> Iterate<T>(this Range range, IEnumerable<T> items)
    {
        (int offset, int length) = range.GetOffsetAndLength(items.Count());
        return items.Skip(offset).Take(length);
    }
}