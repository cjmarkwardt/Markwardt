namespace Markwardt;

public interface IServiceInitializeMap
{
    Type Service { get; }
    IReadOnlyDictionary<string, IServiceDependency> Dependencies { get; }
}

public class ServiceInitializeMap(Type service) : IServiceInitializeMap
{
    public Type Service => service;
    public IReadOnlyDictionary<string, IServiceDependency> Dependencies { get; } = service.GetMembers().Where(ServiceDependency.IsDependency).ToDictionary(x => x.Name, x => (IServiceDependency)new ServiceDependency(x));
}
