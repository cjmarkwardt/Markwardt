namespace Markwardt;

public class ServiceInstantiator(Type implementation, Func<Type, MethodBase?>? methodLocator = null) : IServiceBuilder
{
    private static MethodBase? LocateMethod(Type type)
    {
        bool IsCopyConstructor(ConstructorInfo constructor)
            => constructor.GetParameters().Length == 1 && constructor.GetParameters()[0].ParameterType == type;

        IEnumerable<MethodBase> methods = type.GetConstructors().Where(c => !IsCopyConstructor(c)).Cast<MethodBase>().Concat(type.GetMethods(BindingFlags.Static | BindingFlags.Public));

        MethodBase? attributed = methods.FirstOrDefault(m => m.GetCustomAttribute<FactoryTargetAttribute>() != null);
        if (attributed != null)
        {
            return attributed;
        }

        MethodBase? staticCreate = methods.OfType<MethodInfo>().Where(m => m.Name == "Create").OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();
        if (staticCreate != null)
        {
            return staticCreate;
        }

        MethodBase? longestConstructor = methods.OfType<ConstructorInfo>().OrderByDescending(m => m.GetParameters().Length).FirstOrDefault();
        if (longestConstructor != null)
        {
            return longestConstructor;
        }

        return null;
    }

    private readonly Func<Type, MethodBase?> methodLocator = methodLocator ?? LocateMethod;
    private readonly TypeGeneralizer generalizer = new(implementation);
    private readonly Dictionary<IReadOnlyDictionary<string, Type>, ServiceConstructor> invokers = new(new TypeArgumentComparer());

    public async ValueTask<object> Build(IServiceResolver services, IReadOnlyDictionary<string, object?>? arguments = null)
    {
        IReadOnlyDictionary<string, Type> typeArguments = generalizer.GetTypeArguments(arguments);
        if (!invokers.TryGetValue(typeArguments, out ServiceConstructor? constructor))
        {
            Type type = generalizer.Specify(typeArguments);
            constructor = new ServiceConstructor(methodLocator(type).NotNull($"Unable to find construction method for {type}"));
            invokers.Add(typeArguments, constructor);
        }

        return await constructor.Build(services, arguments);
    }
}
