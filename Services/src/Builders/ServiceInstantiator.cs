namespace Markwardt;

public class ServiceInstantiator(Type implementation, Func<Type, MethodBase?>? methodLocator = null) : IServiceBuilder
{
    private static MethodBase? LocateMethod(Type type)
    {
        bool IsCopyConstructor(ConstructorInfo constructor)
            => constructor.GetParameters().Length == 1 && constructor.GetParameters().First().ParameterType == type;

        IEnumerable<MethodBase> methods = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(c => !IsCopyConstructor(c)).Cast<MethodBase>().Concat(type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic));

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

    public async ValueTask<object> Build(IServiceResolver resolver, IReadOnlyDictionary<string, object?>? incomingArguments = null)
    {
        IReadOnlyDictionary<string, Type> typeArguments = generalizer.GetTypeArguments(incomingArguments);
        if (!invokers.TryGetValue(typeArguments, out ServiceConstructor? constructor))
        {
            constructor = new ServiceConstructor(methodLocator(generalizer.Specify(typeArguments)).NotNull());
            invokers.Add(typeArguments, constructor);
        }

        return await constructor.Build(resolver, incomingArguments);
    }
}
