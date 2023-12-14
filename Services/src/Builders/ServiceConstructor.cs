namespace Markwardt;

public class ServiceConstructor(MethodBase method) : IServiceBuilder
{
    private readonly MethodGeneralizer generalizer = new(method);
    private readonly Dictionary<IReadOnlyDictionary<string, Type>, Invocation> invocations = new(new TypeArgumentComparer());

    public async ValueTask<object> Build(IServiceResolver resolver, IReadOnlyDictionary<string, object?>? arguments = null)
    {
        IReadOnlyDictionary<string, Type> typeArguments = generalizer.GetTypeArguments(arguments);
        if (!invocations.TryGetValue(typeArguments, out Invocation? invocation))
        {
            invocation = new Invocation(generalizer.Specify(typeArguments));
            invocations.Add(typeArguments, invocation);
        }

        return await invocation.Invoke(resolver, arguments);
    }

    private class Invocation
    {
        private delegate ValueTask<object> InvokeTarget(object?[]? arguments);
        
        public Invocation(MethodBase method)
        {
            this.method = method;
            target = GenerateTarget();
        }

        private readonly MethodBase method;
        private readonly InvokeTarget target;

        public async ValueTask<object> Invoke(IServiceResolver resolver, IReadOnlyDictionary<string, object?>? arguments)
            => await target.Invoke(await ResolveParameters(resolver, arguments));

        private InvokeTarget GenerateTarget()
        {
            ParameterExpression injectedArguments = Expression.Parameter(typeof(object?[]));
            
            IEnumerable<Expression> CreateArguments(MethodBase method)
                => method.GetParameters().Select((p, i) => Expression.Convert(Expression.ArrayIndex(injectedArguments, Expression.Constant(i)), p.ParameterType));

            Expression body;
            if (method is ConstructorInfo constructor)
            {
                body = Expression.New(typeof(ValueTask<object>).GetConstructor(new Type[] { typeof(object) })!, Expression.New(constructor, CreateArguments(constructor)));
            }
            else if (method is MethodInfo directMethod)
            {
                Type returnType = directMethod.ReturnType;

                if (!directMethod.IsStatic)
                {
                    throw new InvalidOperationException($"Method {method} must be static");
                }
                else if (returnType == typeof(void) || returnType == typeof(Task) || returnType == typeof(ValueTask))
                {
                    throw new InvalidOperationException($"Method {method} must return a result");
                }

                if (returnType.TryGetGenericTypeDefinition() == typeof(ValueTask<>))
                {
                    body = Expression.Call(typeof(TaskExtensions), nameof(TaskExtensions.Generalize), [directMethod.ReturnType.GetGenericArguments().First()], Expression.Call(directMethod, CreateArguments(directMethod)));
                }
                else if (returnType.TryGetGenericTypeDefinition() == typeof(Task<>))
                {
                    throw new NotImplementedException($"Method {method} must return a ValueTask instead of a Task");
                }
                else
                {
                    body = Expression.New(typeof(ValueTask<object>).GetConstructor([typeof(object)])!, Expression.Call(directMethod, CreateArguments(directMethod)));
                }
            }
            else
            {
                throw new InvalidOperationException();
            }

            return Expression.Lambda<InvokeTarget>(body, injectedArguments).Compile();
        }

        private async ValueTask<object?[]?> ResolveParameters(IServiceResolver resolver, IReadOnlyDictionary<string, object?>? arguments)
        {
            IReadOnlyList<ParameterInfo> parameters = method.GetParameters();
            if (!parameters.Any())
            {
                return null;
            }

            object?[] resolvedArguments = new object?[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                resolvedArguments[i] = await ResolveParameter(resolver, arguments, parameters[i]);
            }

            return resolvedArguments;
        }

        private static async ValueTask<object?> ResolveParameter(IServiceResolver resolver, IReadOnlyDictionary<string, object?>? arguments, ParameterInfo parameter)
        {
            if (arguments != null && arguments.TryGetValue(parameter.Name!.ToLower(), out object? argument))
            {
                return argument;
            }
            else if (parameter.TryGetCustomAttribute(out BaseInjectAttribute? injectAttribute) && (await resolver.Resolve(injectAttribute.GetKey(parameter.ParameterType))).TryNotNull(out object? injectInstance))
            {
                return injectInstance.Convert(parameter.ParameterType);
            }
            else if ((await resolver.Resolve(parameter.ParameterType)).TryNotNull(out object? defaultInstance))
            {
                return defaultInstance;
            }
            else if (parameter.HasDefaultValue)
            {
                return parameter.DefaultValue;
            }
            else
            {
                throw new InvalidOperationException($"Unable to resolve parameter {parameter}");
            }
        }
    }
}