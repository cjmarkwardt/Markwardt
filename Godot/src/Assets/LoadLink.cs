namespace Markwardt.Godot;

public interface ILoadLink
{
    ValueTask<Failable<object>> TryLoad();
}

public interface ILoadLink<T> : ILoadLink
{
    new ValueTask<Failable<T>> TryLoad();
}

public static class LoadLinkExtensions
{
    public static async ValueTask<object> Load(this ILoadLink link)
        => (await link.TryLoad()).Result;

    public static async ValueTask<T> Load<T>(this ILoadLink<T> link)
        => (await link.TryLoad()).Result;

    public static async ValueTask<Failable<T>> TryLoad<TConstructor, T>(this ILoadLink<TConstructor> link, AsyncFunc<TConstructor, T> construct)
    {
        Failable<TConstructor> tryLoad = await link.TryLoad();
        if (tryLoad.Exception != null)
        {
            return tryLoad.Exception;
        }

        return await construct(tryLoad.Result);
    }
}

public abstract class LoadLink<T>(ILoader loader, string path) : ILoadLink<T>
{
    public async ValueTask<Failable<T>> TryLoad()
        => await loader.TryLoad<T>(path);

    async ValueTask<Failable<object>> ILoadLink.TryLoad()
        => await loader.TryLoad(path, typeof(T));
}