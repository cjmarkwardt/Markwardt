namespace Markwardt;

public interface IServiceContainer : IServiceResolver, IServiceConfiguration, IExtendedDisposable { }

public class ServiceContainer : ExtendedDisposable, IServiceContainer
{
    public ServiceContainer(IServiceHandler? handler = null, IServiceResolver? parent = null)
    {
        this.handler = handler;
        this.parent = parent;
    }

    public ServiceContainer(IServiceResolver parent)
    {
        this.parent = parent;
    }

    private readonly IServiceHandler? handler;
    private readonly IServiceResolver? parent;
    private readonly Dictionary<object, Registration> registrations = [];

    private TaskCompletionSource<ITopLogger>? topLogger;

    public void Configure(object key, IServiceDescription description)
    {
        Clear(key);
        registrations[key] = Add(key, description);
    }

    public void Clear(object key)
    {
        if (registrations.TryGetValue(key, out Registration? registration))
        {
            if (!registration.CanReplace)
            {
                throw new InvalidOperationException($"Service key {key} cannot be replaced");
            }
            
            registrations.Remove(key);
        }
    }

    public async ValueTask<object?> Resolve(object key)
    {
        if (topLogger is null)
        {
            topLogger = new();
            topLogger.SetResult(await this.RequireDefault<ITopLogger>());
        }

        if (registrations.TryGetValue(key, out Registration? registration))
        {
            return await registration.Get(this);
        }
        else if (typeof(IServiceResolver).Equals(key) || typeof(IServiceConfiguration).Equals(key) || typeof(IServiceContainer).Equals(key))
        {
            return this;
        }
        else if (typeof(IServiceScopeFactory).Equals(key))
        {
            return new ServiceScopeFactory(this);
        }
        else if (handler != null && handler.Get(key).TryNotNull(out IServiceDescription? description))
        {
            return await Add(key, description).Get(this);
        }
        else if (parent != null)
        {
            return await parent.Resolve(key);
        }
        else if (ServiceTag.GetDefault(key, out IServiceDescription? tagDefault))
        {
            return await Add(key, tagDefault).Get(this);
        }
        else
        {
            return null;
        }
    }

    protected override void OnDisposal()
    {
        base.OnDisposal();

        foreach (Registration registration in registrations.Values)
        {
            registration.Dispose();
        }

        registrations.Clear();
    }

    protected override async ValueTask OnAsyncDisposal()
    {
        await base.OnAsyncDisposal();

        await Task.WhenAll(registrations.Values.Select(x => x.DisposeAsync().AsTask()));
        registrations.Clear();
    }

    private Registration Add(object key, IServiceDescription description)
    {
        Registration registration = new(this, description);
        registrations[key] = registration;
        return registration;
    }

    private sealed class Registration(ServiceContainer container, IServiceDescription description) : IDisposable, IAsyncDisposable
    {
        private object? instance;

        public bool CanReplace => instance == null;

        public async ValueTask<object> Get(IServiceResolver resolver)
        {
            if (description.Kind == ServiceKind.Singleton)
            {
                if (instance is null)
                {
                    instance = await Create(resolver);

                    if (container.topLogger?.Task.IsCompleted == true)
                    {
                        instance.RouteLogsTo(await container.topLogger.Task);
                    }
                }
                
                return instance;
            }
            else
            {
                return await Create(resolver);
            }
        }

        public void Dispose()
        {
            if (instance is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (instance is IAsyncDisposable disposable)
            {
                await disposable.DisposeAsync();
            }
        }

        private async ValueTask<object> Create(IServiceResolver resolver)
            => await description.Builder.Build(resolver);
    }
}