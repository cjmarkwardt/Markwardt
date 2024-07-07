namespace Markwardt;

public interface IAssetSource
{
    IAsyncEnumerable<KeyValuePair<string, Failable<IDisposable<object>>>> Claim(IEnumerable<string> keys);
}

public static class AssetSourceExtensions
{
    public static async IAsyncEnumerable<KeyValuePair<string, Failable<IDisposable<T>>>> Claim<T>(this IAssetSource source, IEnumerable<string> keys, Func<object, ValueTask<Failable<T>>> selector)
    {
        await foreach (KeyValuePair<string, Failable<IDisposable<object>>> claim in source.Claim(keys))
        {
            Failable<IDisposable<T>> result;

            if (claim.Value.Exception is not null)
            {
                result = claim.Value.Exception;
            }
            else
            {
                Failable<T> trySelect = await selector(claim.Value.Result.Value);
                if (trySelect.Exception is not null)
                {
                    result = trySelect.Exception;
                    await claim.Value.Result.DisposeAsync();
                }
                else
                {
                    result = new Disposable<T>(trySelect.Result, [claim.Value.Result]);
                }
            }

            yield return new KeyValuePair<string, Failable<IDisposable<T>>>(claim.Key, result);
        }
    }

    public static IAsyncEnumerable<KeyValuePair<string, Failable<IDisposable<T>>>> Claim<T>(this IAssetSource source, IEnumerable<string> keys)
        => source.Claim(keys, x => ValueTask.FromResult(x is T casted ? casted : Failable.Fail<T>($"Asset {x} is not of type {typeof(T)}")));

    public static async ValueTask<Failable<IDisposable<object>>> Claim(this IAssetSource source, string key)
        => await source.Claim([key]).Select(x => x.Value).FirstAsync();

    public static async ValueTask<Failable<IDisposable<T>>> Claim<T>(this IAssetSource source, string key, Func<object, ValueTask<Failable<T>>> selector)
        => await source.Claim([key], selector).Select(x => x.Value).FirstAsync();

    public static async ValueTask<Failable<IDisposable<T>>> Claim<T>(this IAssetSource source, string key)
        => await source.Claim<T>([key]).Select(x => x.Value).FirstAsync();
        
    public static async ValueTask Access<T>(this IAssetSource source, IEnumerable<string> keys, Func<T, ValueTask> access)
    {
        await foreach (IDisposable<T> claim in source.Claim<T>(keys).Where(x => x.Value.Exception is null).Select(x => x.Value.Result))
        {
            await using IDisposable<T> target = claim;
            await access(target.Value);
        }
    }

    public static async ValueTask Access<T>(this IAssetSource source, IEnumerable<string> keys, Action<T> access)
        => await source.Access<T>(keys, x => { access(x); return ValueTask.CompletedTask; });

    public static async ValueTask Access<T>(this IAssetSource source, string key, Func<T, ValueTask> access)
        => await source.Access([key], access);

    public static async ValueTask Access<T>(this IAssetSource source, string key, Action<T> access)
        => await source.Access([key], access);

    public static async ValueTask Activate(this IAssetSource source, IEnumerable<string> keys)
        => await source.Access<IAssetTrigger>(keys, async x => await x.Activate());

    public static async ValueTask Activate(this IAssetSource source, string key)
        => await source.Activate([key]);
}