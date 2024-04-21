namespace Markwardt;

public interface IServiceDescription
{
    ServiceKind Kind { get; }
    IServiceBuilder Builder { get; }
}

public static class ServiceDescriptionExtensions
{
    public static async ValueTask<object> Create(this IServiceDescription description, IServiceResolver resolver, IReadOnlyDictionary<string, object?>? arguments = null)
        => await description.Builder.Build(resolver, arguments);
    
    public static async ValueTask<T> Create<T>(this IServiceDescription description, IServiceResolver resolver, IReadOnlyDictionary<string, object?>? arguments = null)
        => (T) await description.Create(resolver, arguments);
}

public record ServiceDescription(ServiceKind Kind, IServiceBuilder Builder) : IServiceDescription;