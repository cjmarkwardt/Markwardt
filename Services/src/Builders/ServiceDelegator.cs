namespace Markwardt;

public static class ServiceDelegatorExtensions
{
    public static IServiceBuilder ThroughDelegate(this IServiceBuilder builder, Type @delegate)
        => !@delegate.IsDelegate() ? builder : new ServiceDelegator(builder, @delegate);
}

public class ServiceDelegator : IServiceBuilder
{
    public ServiceDelegator(IServiceBuilder builder, Type @delegate)
    {
        this.builder = builder;

        if (!@delegate.AsDelegate(out DelegateType? type))
        {
            throw new InvalidOperationException($"Type {@delegate} is not a delegate");
        }
        else if (type.Return.TryGetGenericTypeDefinition() != typeof(ValueTask<>))
        {
            throw new InvalidOperationException($"Delegate {@delegate} must return a ValueTask<T>");
        }

        generalizer = new(@delegate);
    }

    private readonly IServiceBuilder builder;
    private readonly TypeGeneralizer generalizer;
    private readonly Dictionary<IReadOnlyDictionary<string, Type>, Delegate> delegates = new(new TypeArgumentComparer());

    public ValueTask<object> Build(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments = null)
    {
        IReadOnlyDictionary<string, Type> typeArguments = generalizer.GetTypeArguments(arguments);
        if (!delegates.TryGetValue(typeArguments, out Delegate? @delegate))
        {
            @delegate = generalizer.Specify(typeArguments).AsDelegate()!.Implement(arguments => builder.Build(services, arguments)!);
            delegates.Add(typeArguments, @delegate);
        }

        return new(@delegate);
    }
}