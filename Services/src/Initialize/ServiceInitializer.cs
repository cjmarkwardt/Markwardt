namespace Markwardt;

[Singleton<ServiceInitializer>]
public interface IServiceInitializer
{
    IServiceInitializeMap GetMap(object service);
}

public static class ServiceInitializerExtensions
{
    public static async ValueTask<bool> TryAutoInitialize(this IServiceInitializer initializer, object service, IServiceResolver resolver)
    {
        if (!InitializeFlag.IsInitialized(service))
        {
            foreach (IServiceDependency dependency in initializer.GetMap(service).Dependencies.Values)
            {
                await dependency.AutoInject(service, resolver);
            }

            InitializeFlag.SetInitialized(service);
            return true;
        }

        return false;
    }

    public static async ValueTask AutoInitialize(this IServiceInitializer initializer, object service, IServiceResolver resolver)
    {
        if (! await initializer.TryAutoInitialize(service, resolver))
        {
            throw new InvalidOperationException("Service is already initialized");
        }
    }
}

public class ServiceInitializer : IServiceInitializer
{
    private readonly Dictionary<Type, IServiceInitializeMap> maps = [];

    public IServiceInitializeMap GetMap(object service)
    {
        Type type = service.GetType();
        if (!maps.TryGetValue(type, out IServiceInitializeMap? map))
        {
            map = new ServiceInitializeMap(type);
        }

        return map;
    }
}