namespace Markwardt;

public static class ListExtensions
{
    public static Maybe<T> TryDequeue<T>(this IList<T> list)
        => list.Count == 0 ? default : list[0].Maybe();

    public static IList<T> TryDequeue<T>(this IManyList<T> list, int count)
    {
        Range range = new(Index.FromEnd(count), Index.End);
        IList<T> items = list.Get(range);
        list.RemoveAt(range);
        return items;
    }

    public static T Dequeue<T>(this IList<T> list)
        => list.TryDequeue().Value;
    
    public static IList<T> Dequeue<T>(this IManyList<T> list, int count)
    {
        IList<T> items = list.TryDequeue(count);
        if (items.Count != count)
        {
            throw new InvalidOperationException();
        }

        return items;
    }

    public static Maybe<T> TryPop<T>(this IList<T> list)
        => list.Count == 0 ? default : list[list.Count - 1].Maybe();

    public static IList<T> TryPop<T>(this IManyList<T> list, int count)
    {
        Range range = new(Index.Start, Index.FromStart(count));
        IList<T> items = list.Get(range);
        list.RemoveAt(range);
        return items;
    }

    public static T Pop<T>(this IList<T> list)
        => list.TryPop().Value;
    
    public static IList<T> Pop<T>(this IManyList<T> list, int count)
    {
        IList<T> items = list.TryPop(count);
        if (items.Count != count)
        {
            throw new InvalidOperationException();
        }

        return items;
    }
}