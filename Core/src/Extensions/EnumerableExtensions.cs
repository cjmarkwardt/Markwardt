namespace Markwardt;

public static class EnumerableExtensions
{
    public static bool None<T>(this IEnumerable<T> items)
        => !items.Any();

    public static IEnumerable<TSelected> SelectMaybe<T, TSelected>(this IEnumerable<T> items, Func<T, Maybe<TSelected>> select)
        => items.Select(x => select(x)).Where(x => x.HasValue).Select(x => x.Value);

    public static bool MinSequenceEqual<T>(this IEnumerable<T> items, IEnumerable<T> other, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;

        IEnumerator<T> itemsEnumerator = items.GetEnumerator();
        IEnumerator<T> otherEnumerator = other.GetEnumerator();
        while (itemsEnumerator.Next().TryGetValue(out T? item))
        {
            Maybe<T> maybeOtherItem = otherEnumerator.Next();
            if (!maybeOtherItem.HasValue || (maybeOtherItem.TryGetValue(out T? otherItem) && !comparer.Equals(item, otherItem)))
            {
                return false;
            }
        }

        return true;
    }

    public static IEnumerable<T> When<T>(this IEnumerable<T> items, Func<IEnumerable<T>, bool> condition, Func<IEnumerable<T>, IEnumerable<T>> transform)
        => condition(items) ? transform(items) : items;

    public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (T item in items)
        {
            action(item);
        }
    }

    public static async ValueTask ForEach<T>(this IEnumerable<T> items, Func<T, ValueTask> action)
    {
        foreach (T item in items)
        {
            await action(item);
        }
    }

    public static async ValueTask ForEachParallel<T>(this IEnumerable<T> items, Func<T, ValueTask> action)
        => await Task.WhenAll(items.Select(x => action(x).AsTask()).ToArray());

    public static IEnumerable Cast(this IEnumerable items, Type type)
        => (IEnumerable)Reflector.Reflect(Enumerable.Cast<object?>, type).Invoke(null, [items])!;
}