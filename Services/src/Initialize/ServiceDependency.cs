namespace Markwardt;

public interface IServiceDependency
{
    string Name { get; }
    Type Type { get; }
    bool IsOptional { get; }
    object? Key { get; }

    void Inject(object service, object instance);
}

public static class ServiceDependencyExtensions
{
    public static async ValueTask AutoInject(this IServiceDependency dependency, object service, IServiceResolver resolver)
    {
        object key = dependency.Key ?? dependency.Type;
        object? instance = dependency.IsOptional ? await resolver.Resolve(key) : await resolver.Require(key);
        if (instance != null)
        {
            dependency.Inject(service, await resolver.Require(dependency.Key ?? dependency.Type));
        }
    }
}

public class ServiceDependency : IServiceDependency
{
    public static bool IsDependency(MemberInfo member)
        => member.HasCustomAttribute<BaseInjectAttribute>() && (member is PropertyInfo || member is FieldInfo);

    public ServiceDependency(MemberInfo member)
    {
        Name = member.Name;
        Type = member is PropertyInfo property ? property.PropertyType : ((FieldInfo)member).FieldType;

        BaseInjectAttribute injectAttribute = member.GetCustomAttribute<BaseInjectAttribute>().NotNull();
        IsOptional = injectAttribute.IsOptional;
        Key = injectAttribute.GetKey(Type);

        ParameterExpression service = Expression.Parameter(typeof(object), nameof(service));
        ParameterExpression instance = Expression.Parameter(typeof(object), nameof(instance));
        inject = Expression.Lambda<Action<object, object>>(Expression.Assign(Expression.PropertyOrField(Expression.Convert(service, member.DeclaringType.NotNull()), member.Name), Expression.Convert(instance, Type)), service, instance).Compile();
    }

    private readonly Action<object, object> inject;

    public string Name { get; }
    public Type Type { get; }
    public bool IsOptional { get; }
    public object? Key { get; }

    public void Inject(object service, object instance)
        => inject(service, instance);
}