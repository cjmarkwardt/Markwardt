namespace Markwardt;

public static class Service
{
    public static IServiceDescription FromBuilder(ServiceKind kind, IServiceBuilder builder)
        => new ServiceDescription(kind, builder);
    
    public static IServiceDescription FromInstance(bool dispose, object instance)
        => new ServiceDescription(dispose ? ServiceKind.Singleton : ServiceKind.Transient, new ServiceInstance(instance));

    public static IServiceDescription FromDelegate(ServiceKind kind, Func<IServiceResolver, IReadOnlyDictionary<string, object?>?, ValueTask<object>> function)
        => FromBuilder(kind, new ServiceBuilder(function));

    public static IServiceDescription FromDelegate(ServiceKind kind, Func<IServiceResolver, ValueTask<object>> function)
        => FromDelegate(kind, async (resolver, _) => await function(resolver));

    public static IServiceDescription FromDelegate(ServiceKind kind, Func<ValueTask<object>> function)
        => FromDelegate(kind, async _ => await function());

    public static IServiceDescription FromDelegate(ServiceKind kind, Func<IServiceResolver, IReadOnlyDictionary<string, object?>?, object> function)
        => FromDelegate(kind, (resolver, arguments) => ValueTask.FromResult(function(resolver, arguments)));

    public static IServiceDescription FromDelegate(ServiceKind kind, Func<IServiceResolver, object> function)
        => FromDelegate(kind, resolver => ValueTask.FromResult(function(resolver)));

    public static IServiceDescription FromDelegate(ServiceKind kind, Func<object> function)
        => FromDelegate(kind, () => ValueTask.FromResult(function()));

    public static IServiceDescription FromMethod(ServiceKind kind, MethodBase method)
        => FromBuilder(kind, new ServiceConstructor(method));

    public static IServiceDescription FromImplementation(ServiceKind kind, Type type)
        => FromBuilder(kind, new ServiceInstantiator(type));

    public static IServiceDescription FromImplementation<T>(ServiceKind kind)
        where T : class
        => FromImplementation(kind, typeof(T));

    public static IServiceDescription FromFactory(Type implementation, Type @delegate)
        => FromBuilder(ServiceKind.Singleton, FromImplementation(ServiceKind.Singleton, implementation).Builder.ThroughDelegate(@delegate));

    public static IServiceDescription FromFactory<TImplementation, TDelegate>()
        where TImplementation : class
        where TDelegate : Delegate
        => FromFactory(typeof(TImplementation), typeof(TDelegate));

    public static IServiceDescription FromKey(object key, IServiceResolver? resolver = null)
        => FromBuilder(ServiceKind.Transient, new ServiceRouter(key, resolver));

    public static IServiceDescription FromDefault<T>(IServiceResolver? resolver = null)
        => FromKey(typeof(T), resolver);

    public static IServiceDescription FromTag<TTag>(IServiceResolver? resolver = null)
        where TTag : IServiceTag
        => FromKey(typeof(TTag), resolver);

    public static IServiceDescription FromSource(ServiceKind kind, object key, Func<object, ValueTask<object>> get)
        => FromDelegate(kind, async provider => await get(await provider.Require(key)));

    public static IServiceDescription FromSource(ServiceKind kind, object key, Func<object, object> get)
        => FromSource(kind, key, x => ValueTask.FromResult(get(x)));

    public static IServiceDescription FromSourceDefault<TSource>(ServiceKind kind, Func<TSource, ValueTask<object>> get)
        => FromSource(kind, typeof(TSource), async source => await get((TSource)source));

    public static IServiceDescription FromSourceDefault<TSource>(ServiceKind kind, Func<TSource, object> get)
        => FromSourceDefault<TSource>(kind, x => ValueTask.FromResult(get(x)));

    public static IServiceDescription FromSourceTag<TSourceTag, TSource>(ServiceKind kind, Func<TSource, ValueTask<object>> get)
        => FromSource(kind, typeof(TSourceTag), async source => await get((TSource)source));

    public static IServiceDescription FromSourceTag<TSourceTag, TSource>(ServiceKind kind, Func<TSource, object> get)
        => FromSourceTag<TSourceTag, TSource>(kind, x => ValueTask.FromResult(get(x)));
}