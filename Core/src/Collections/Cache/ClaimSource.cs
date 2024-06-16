namespace Markwardt;

public interface IClaimSource<TKey, T>
{
    IAsyncEnumerable<KeyValuePair<TKey, IDisposable<T>>> Claim(IEnumerable<TKey> keys);
}

public static class ClaimSourceExtensions
{
    public static async ValueTask<Maybe<IDisposable<T>>> Claim<TKey, T>(this IClaimSource<TKey, T> source, TKey key)
        => (await source.Claim([key]).FirstOrMaybeAsync()).Select(x => x.Value);

    public static async IAsyncEnumerable<KeyValuePair<TKey, IDisposable<TSelected>>> Claim<TKey, T, TSelected>(this IClaimSource<TKey, T> source, IEnumerable<TKey> keys, Func<T, Maybe<TSelected>> selector)
        where TKey : notnull
    {
        if (!keys.Any())
        {
            yield break;
        }

        await foreach (KeyValuePair<TKey, IDisposable<T>> claim in source.Claim(keys))
        {
            Maybe<TSelected> selected = selector(claim.Value.Value);
            if (selected.HasValue)
            {
                yield return new(claim.Key, claim.Value.Select(_ => selected.Value));
            }
            else
            {
                await claim.Value.DisposeAsync();
            }
        }
    }

    public static async ValueTask<Maybe<IDisposable<TSelected>>> Claim<TKey, T, TSelected>(this IClaimSource<TKey, T> source, TKey key, Func<T, Maybe<TSelected>> selector)
        where TKey : notnull
    {
        Maybe<IDisposable<T>> claim = await source.Claim(key);
        if (!claim.HasValue)
        {
            return default;
        }

        Maybe<TSelected> selected = selector(claim.Value.Value);
        if (!selected.HasValue)
        {
            await claim.Value.DisposeAsync();
            return default;
        }

        return claim.Value.Select(_ => selected.Value).Maybe();
    }

    public static IAsyncEnumerable<KeyValuePair<TKey, IDisposable<TSelected>>> Claim<TKey, T, TSelected>(this IClaimSource<TKey, T> source, IEnumerable<TKey> keys)
        where TKey : notnull
        => source.Claim(keys, x => x is TSelected selected ? selected : new Maybe<TSelected>());

    public static async ValueTask<Maybe<IDisposable<TSelected>>> Claim<TKey, T, TSelected>(this IClaimSource<TKey, T> source, TKey key)
        where TKey : notnull
        => await source.Claim(key, x => x is TSelected selected ? selected : new Maybe<TSelected>());
}