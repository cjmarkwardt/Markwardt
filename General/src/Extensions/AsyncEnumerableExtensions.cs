namespace Markwardt;

public static class AsyncEnumerableExtensions
{
    public static async ValueTask<Failable<IEnumerable<T>>> Consolidate<T>(this IAsyncEnumerable<Failable<T>> items, CancellationToken cancellation = default)
    {
        List<T> consolidated = [];
        await foreach (Failable<T> tryNext in items)
        {
            if (tryNext.Exception != null)
            {
                return tryNext.Exception.AsFailable<IEnumerable<T>>();
            }

            consolidated.Add(tryNext.Result);
        }

        return consolidated;
    }
}