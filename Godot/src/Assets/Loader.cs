namespace Markwardt.Godot;

public interface ILoader
{
    ValueTask<Failable<object>> TryLoad(string path, Type? type = null);
    void ClearCache();
}

public static class LoaderExtensions
{
    public static async ValueTask<Failable<T>> TryLoad<T>(this ILoader loader, string path)
        => (await loader.TryLoad(path, typeof(T))).Cast<T>();

    public static async ValueTask<Failable<T>> TryLoad<TConstructor, T>(this ILoader loader, string path, AsyncFunc<TConstructor, T> construct)
    {
        Failable<TConstructor> tryLoad = await loader.TryLoad<TConstructor>(path);
        if (tryLoad.Exception != null)
        {
            return tryLoad.Exception;
        }

        return await construct(tryLoad.Result);
    }

    public static async ValueTask<object> Load(this ILoader loader, string path, Type? type = null)
        => (await loader.TryLoad(path, type)).Result;

    public static async ValueTask<T> Load<T>(this ILoader loader, string path)
        => (await loader.TryLoad<T>(path)).Result;

    public static async ValueTask<T> Load<TConstructor, T>(this ILoader loader, string path, AsyncFunc<TConstructor, T> construct)
        => (await loader.TryLoad(path, construct)).Result;
}

public abstract class Loader : ILoader
{
    public abstract ValueTask<Failable<object>> TryLoad(string path, Type? type = null);

    public void ClearCache() { }
}