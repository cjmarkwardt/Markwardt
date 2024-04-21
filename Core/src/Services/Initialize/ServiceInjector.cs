namespace Markwardt;

[Singleton<ServiceInjector>]
public interface IServiceInjector
{
    ValueTask Inject(object instance, IServiceResolver resolver);
}

public class ServiceInjector : IServiceInjector
{
    private readonly Dictionary<Type, IEnumerable<IServiceInjector>> maps = [];

    public async ValueTask Inject(object instance, IServiceResolver resolver)
    {
        Type type = instance.GetType();
        if (!maps.TryGetValue(type, out IEnumerable<IServiceInjector>? map))
        {
            map = ServiceMemberInjector.Generate(type);
            maps.Add(type, map);
        }

        foreach (IServiceInjector dependency in map)
        {
            await dependency.Inject(instance, resolver);
        }
    }
}