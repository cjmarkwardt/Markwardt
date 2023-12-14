namespace Markwardt;

public static class DelegateTypeExtensions
{
    public static DelegateType? AsDelegate(this Type type)
        => DelegateType.Create(type);

    public static bool AsDelegate(this Type type, [NotNullWhen(true)] out DelegateType? delegateType)
        => DelegateType.Create(type, out delegateType);
}

public class DelegateType
{
    public static DelegateType? Create(Type type)
        => type.IsDelegate() ? new DelegateType(type) : null;

    public static bool Create(Type type, [NotNullWhen(true)] out DelegateType? delegateType)
    {
        delegateType = Create(type);
        return delegateType != null;
    }

    private DelegateType(Type type)
    {
        Type = type;

        invoke = new(() => type.GetMethod(nameof(Action.Invoke))!);
        parameters = new(invoke.Value.GetParameters);
    }

    private readonly Lazy<MethodInfo> invoke;
    private readonly Lazy<IReadOnlyList<ParameterInfo>> parameters;

    public Type Type { get; }

    public Type Return => invoke.Value.ReturnType;
    public IReadOnlyList<ParameterInfo> Parameters => parameters.Value;

    public delegate object? Implementation(IReadOnlyDictionary<string, object?> arguments);
    public delegate ValueTask<object?> AsyncImplementation(IReadOnlyDictionary<string, object?> arguments);

    public DelegateType Close(IReadOnlyDictionary<string, Type>? typeArguments)
    {
        if (!Type.IsGenericTypeDefinition)
        {
            return this;
        }

        IReadOnlyDictionary<string, Type> typeArgumentLookup = typeArguments ?? new Dictionary<string, Type>();
        Type[] types = Type.GetGenericArguments();
        for (int i = 0; i < types.Length; i++)
        {
            if (typeArgumentLookup.TryGetValue(types[i].Name.ToLower(), out Type? type))
            {
                types[i] = type;
            }
            else
            {
                throw new InvalidOperationException($"No type argument given for type parameter {types[i]} in delegate {Type}");
            }
        }

        return new DelegateType(Type.MakeGenericType(types));
    }

    public Delegate Implement(Expression<Implementation> implementation)
    {
        if (Type.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException($"Delegate {Type} must be closed");
        }

        IEnumerable<ParameterExpression> parameters = GenerateParameters();
        return Expression.Lambda(Type, Expression.Convert(Expression.Invoke(implementation, GenerateArguments(parameters)), Return), parameters).Compile();
    }

    public Delegate Implement(Expression<AsyncImplementation> implementation)
    {
        if (Type.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException($"Delegate {Type} must be closed");
        }

        IEnumerable<ParameterExpression> parameters = GenerateParameters();
        return Expression.Lambda(Type, Expression.Call(typeof(TaskExtensions), nameof(TaskExtensions.Specify), [Return.GetGenericArguments().First()], Expression.Invoke(implementation, GenerateArguments(parameters))), parameters).Compile();
    }

    private IEnumerable<ParameterExpression> GenerateParameters()
        => !Parameters.Any() ? Enumerable.Empty<ParameterExpression>() : Parameters.Select(p => Expression.Parameter(p.ParameterType, p.Name)).ToList();

    private static Expression GenerateArguments(IEnumerable<ParameterExpression> parameters)
    {
        Type dictionaryType = typeof(Dictionary<string, object?>);
        ParameterExpression arguments = Expression.Variable(dictionaryType, nameof(arguments));
        List<Expression> body =
        [
            Expression.Assign(arguments, Expression.New(dictionaryType)),
            .. parameters.Select(p => Expression.Call(arguments, dictionaryType.GetMethod("Add").NotNull(), Expression.Constant(p.Name), Expression.Convert(p, typeof(object)))),
            arguments,
        ];
        return Expression.Block(dictionaryType, new[] { arguments }, body);
    }
}