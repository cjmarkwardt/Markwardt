namespace Markwardt;

public interface IServiceResolver : IMultiDisposable
{
    ValueTask<object?> Resolve(object key);
}

public static class ServiceResolverExtensions
{
    public static async ValueTask<object> Require(this IServiceResolver resolver, object key)
        => await resolver.Resolve(key) ?? throw new InvalidOperationException($"Could not resolve service for key {key}");

    public static async ValueTask<T?> Resolve<T>(this IServiceResolver resolver, object key)
        where T : class
        => (T?)await resolver.Resolve(key);

    public static async ValueTask<T> Require<T>(this IServiceResolver resolver, object key)
        where T : class
        => (T)await resolver.Require(key);

    public static async ValueTask<T?> ResolveDefault<T>(this IServiceResolver resolver)
        where T : class
        => (T?)await resolver.Resolve(typeof(T));

    public static async ValueTask<T> RequireDefault<T>(this IServiceResolver resolver)
        where T : class
        => (T)await resolver.Require(typeof(T));

    public static async ValueTask<T?> ResolveTag<TTag, T>(this IServiceResolver resolver)
        where TTag : IServiceTag
        where T : class
        => (T?)await resolver.Resolve(typeof(TTag));

    public static async ValueTask<T> RequireTag<TTag, T>(this IServiceResolver resolver)
        where TTag : IServiceTag
        where T : class
        => (T)await resolver.Require(typeof(TTag));
}