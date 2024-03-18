namespace Markwardt;

public interface IServiceConfiguration
{
    void Configure(object key, IServiceDescription description);
    void Clear(object key);
}

public static class ServiceConfigurationExtensions
{
    public static void Configure(this IServiceConfiguration services, IServicePackage package)
        => package.Configure(services);

    public static void Configure<TKey>(this IServiceConfiguration services, IServiceDescription description)
        => services.Configure(typeof(TKey), description);
    
    public static void Clear<TKey>(this IServiceConfiguration services)
        => services.Clear(typeof(TKey));
}