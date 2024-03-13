namespace Markwardt;

public static class EnumerableExtensions
{
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