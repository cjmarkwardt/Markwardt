namespace Markwardt;

public record AssetId(string Value)
{
    public async ValueTask<Failable<IDisposable<object>>> Claim(IAssetSource source)
        => await source.Claim(Value);

    public async ValueTask<Failable<IDisposable<T>>> Claim<T>(IAssetSource source, Func<object, ValueTask<Failable<T>>> selector)
        => await source.Claim(Value, selector);
        
    public async ValueTask<Failable<IDisposable<T>>> Claim<T>(IAssetSource source)
        => await source.Claim<T>(Value);
        
    public static async ValueTask Access<T>(IAssetSource source, string key, Func<T, ValueTask> access)
        => await source.Access(key, access);

    public static async ValueTask Access<T>(IAssetSource source, string key, Action<T> access)
        => await source.Access(key, access);

    public static async ValueTask Activate(IAssetSource source, string key)
        => await source.Activate(key);
}

public record AssetId<T>(string Value)
{
    public async ValueTask<Failable<IDisposable<TSelected>>> Claim<TSelected>(IAssetSource source, Func<object, ValueTask<Failable<TSelected>>> selector)
        => await source.Claim(Value, selector);
        
    public async ValueTask<Failable<IDisposable<T>>> Claim(IAssetSource source)
        => await source.Claim<T>(Value);
        
    public async ValueTask<Failable<IDisposable<TCasted>>> Claim<TCasted>(IAssetSource source)
        where TCasted : T
        => await source.Claim<TCasted>(Value);
        
    public async ValueTask Access(IAssetSource source, Func<T, ValueTask> access)
        => await source.Access(Value, access);

    public async ValueTask Access(IAssetSource source, Action<T> access)
        => await source.Access(Value, access);

    public async ValueTask Activate(IAssetSource source)
        => await source.Activate(Value);
}