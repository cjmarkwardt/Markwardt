namespace Markwardt;

public class ServiceMemberInjector : IServiceInjector
{
    public static IEnumerable<IServiceInjector> Generate(Type type)
        => type.GetMembers().Where(IsDependency).Select(x => new ServiceMemberInjector(x));

    private static bool IsDependency(MemberInfo member)
        => (member is PropertyInfo property && property.IsInitOnly()) || ((member is PropertyInfo || member is FieldInfo) && member.HasCustomAttribute<BaseInjectAttribute>());

    public ServiceMemberInjector(MemberInfo member)
    {
        Type type;
        if (member is PropertyInfo property)
        {
            type = property.PropertyType;
        }
        else if (member is FieldInfo field)
        {
            type = field.FieldType;
        }
        else
        {
            throw new InvalidOperationException("Dependency must be a property or field");
        }

        if (member.TryGetCustomAttribute(out BaseInjectAttribute? injectAttribute))
        {
            isOptional = injectAttribute.IsOptional;
            key = injectAttribute.GetKey(type);
        }
        else
        {
            isOptional = true;
            key = type;
        }

        ParameterExpression instance = Expression.Parameter(typeof(object), nameof(instance));
        ParameterExpression service = Expression.Parameter(typeof(object), nameof(service));
        inject = Expression.Lambda<Action<object, object>>(Expression.Assign(Expression.PropertyOrField(Expression.Convert(instance, member.DeclaringType.NotNull()), member.Name), Expression.Convert(service, type)), instance, service).Compile();
    }

    private readonly bool isOptional;
    private readonly object key;
    private readonly Action<object, object> inject;

    public async ValueTask Inject(object instance, IServiceResolver resolver)
    {
        object? service = isOptional ? await resolver.Resolve(key) : await resolver.Require(key);
        if (service is not null)
        {
            inject(instance, service);
        }
    }
}