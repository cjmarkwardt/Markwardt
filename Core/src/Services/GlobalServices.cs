namespace Markwardt;

public static class GlobalServices
{
    public static bool IsInitialized => resolver is not null;

    private static IServiceResolver? resolver;
    public static IServiceResolver Resolver => resolver ?? throw new InvalidOperationException("Global services are not initialized");

    public static void Initialize(IServiceResolver resolver)
    {
        if (GlobalServices.resolver is not null)
        {
            throw new InvalidOperationException("Global services are already initialized");
        }

        GlobalServices.resolver = resolver;
    }

    public static async ValueTask Inject(object instance)
        => await Resolver.Inject(instance);
}