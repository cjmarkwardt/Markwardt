namespace Markwardt;

public interface ITypeTarget
{
    object Source { get; }
    string Name { get; }
    ITypeDescription Description { get; }
}

public class TypeTarget(object source, string name, Func<ITypeDescription> getDescription) : ITypeTarget
{
    public TypeTarget(FieldInfo field)
        : this(field, $"{field.DeclaringType?.Name}.{field.Name}", () => new TypeDescription(field.GetNullability())) { }

    public TypeTarget(PropertyInfo property)
        : this(property, $"{property.DeclaringType?.Name}.{property.Name}", () => new TypeDescription(property.GetNullability())) { }

    public TypeTarget(EventInfo @event)
        : this(@event, $"{@event.DeclaringType?.Name}.{@event.Name}", () => new TypeDescription(@event.GetNullability())) { }

    public TypeTarget(ParameterInfo parameter)
        : this(parameter, $"{parameter.Member.DeclaringType?.Name}.{parameter.Member.Name}.{parameter.Name}", () => new TypeDescription(parameter.GetNullability())) { }

    public object Source => source;
    public string Name => name;

    private readonly Lazy<ITypeDescription> description = new(() => getDescription());
    public ITypeDescription Description => description.Value;
}