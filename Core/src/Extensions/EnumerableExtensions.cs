namespace Markwardt;

public static class EnumerableExtensions
{
    public static IAsyncEnumerable<T> Merge<T>(this IAsyncEnumerable<T> source, params IAsyncEnumerable<T>[] others)
        => others.Prepend(source).Merge();
        
    public static IAsyncEnumerable<T> Merge<T>(this IEnumerable<IAsyncEnumerable<T>> sources)
        => AsyncEnumerableEx.Merge(sources.ToArray());

    public static Maybe<T> FirstOrMaybe<T>(this IEnumerable<T> source)
    {
        IEnumerator<T> enumerator = source.GetEnumerator();
        return enumerator.MoveNext() ? enumerator.Current : new Maybe<T>();
    }

    public static async ValueTask<Maybe<T>> FirstOrMaybeAsync<T>(this IAsyncEnumerable<T> source)
    {
        IAsyncEnumerator<T> enumerator = source.GetAsyncEnumerator();
        return await enumerator.MoveNextAsync() ? enumerator.Current : new Maybe<T>();
    }

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

    public static IEnumerable<T> OnEach<T>(this IEnumerable<T> items, Action<T> action)
    {
        foreach (T item in items)
        {
            action(item);
            yield return item;
        }
    }

    public static async IAsyncEnumerable<T> OnEach<T>(this IEnumerable<T> items, Func<T, ValueTask> action)
    {
        foreach (T item in items)
        {
            await action(item);
            yield return item;
        }
    }

    public static async IAsyncEnumerable<T> OnEach<T>(this IAsyncEnumerable<T> items, Action<T> action)
    {
        await foreach (T item in items)
        {
            action(item);
            yield return item;
        }
    }

    public static async IAsyncEnumerable<T> OnEach<T>(this IAsyncEnumerable<T> items, Func<T, ValueTask> action)
    {
        await foreach (T item in items)
        {
            await action(item);
            yield return item;
        }
    }

    public static IEnumerable Cast(this IEnumerable items, Type type)
        => (IEnumerable)Reflector.Reflect(Enumerable.Cast<object?>, type).Invoke(null, [items])!;
}