namespace Markwardt;

public static class NullabilityExtensions
{
    private readonly static NullabilityInfoContext context = new();

    public static NullabilityInfo GetNullability(this FieldInfo field)
        => context.Create(field);

    public static NullabilityInfo GetNullability(this PropertyInfo property)
        => context.Create(property);

    public static NullabilityInfo GetNullability(this ParameterInfo parameter)
        => context.Create(parameter);

    public static NullabilityInfo GetNullability(this EventInfo @event)
        => context.Create(@event);
}