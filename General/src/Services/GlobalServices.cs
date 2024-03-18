namespace Markwardt;

public static class GlobalServices
{
    public static IServiceResolver? Resolver { get; private set; }

    public static void SetResolver(IServiceResolver? resolver)
        => Resolver = resolver;
}