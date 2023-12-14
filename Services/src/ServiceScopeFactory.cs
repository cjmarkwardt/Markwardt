namespace Markwardt;

public interface IServiceScopeFactory
{
    IServiceResolver Create(Action<IServiceConfiguration> configure);
}

public class ServiceScopeFactory(IServiceResolver parent) : IServiceScopeFactory
{
    public IServiceResolver Create(Action<IServiceConfiguration> configure)
    {
        ServiceContainer container = new(parent);
        configure(container);
        return container;
    }
}