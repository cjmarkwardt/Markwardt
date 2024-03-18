namespace Markwardt;

public interface IServiceDependency
{
    string Name { get; }
    Type Type { get; }
    bool IsOptional { get; }
    object Key { get; }

    void Inject(object service, object instance);
}

public static class ServiceDependencyExtensions
{
    public static async ValueTask AutoInject(this IServiceDependency dependency, object service, IServiceResolver resolver)
    {
        object? instance = dependency.IsOptional ? await resolver.Resolve(dependency.Key) : await resolver.Require(dependency.Key);
        if (instance != null)
        {
            dependency.Inject(service, instance);
        }
    }
}

public class ServiceDependency : IServiceDependency
{
    public static bool IsDependency(MemberInfo member)
        => (member is PropertyInfo property && property.IsInitOnly()) || ((member is PropertyInfo || member is FieldInfo) && member.HasCustomAttribute<BaseInjectAttribute>());

    public ServiceDependency(MemberInfo member)
    {
        if (member is PropertyInfo property)
        {
            Type = property.PropertyType;
        }
        else if (member is FieldInfo field)
        {
            Type = field.FieldType;
        }
        else
        {
            throw new InvalidOperationException("Dependency must be a property or field");
        }

        Name = member.Name;

        if (member.TryGetCustomAttribute(out BaseInjectAttribute? injectAttribute))
        {
            IsOptional = injectAttribute.IsOptional;
            Key = injectAttribute.GetKey(Type);
        }
        else
        {
            IsOptional = true;
            Key = Type;
        }

        ParameterExpression service = Expression.Parameter(typeof(object), nameof(service));
        ParameterExpression instance = Expression.Parameter(typeof(object), nameof(instance));
        inject = Expression.Lambda<Action<object, object>>(Expression.Assign(Expression.PropertyOrField(Expression.Convert(service, member.DeclaringType.NotNull()), member.Name), Expression.Convert(instance, Type)), service, instance).Compile();
    }

    private readonly Action<object, object> inject;

    public string Name { get; }
    public Type Type { get; }
    public bool IsOptional { get; }
    public object Key { get; }

    public void Inject(object service, object instance)
        => inject(service, instance);
}