namespace Markwardt;

public interface ITypeDescription
{
    Type Type { get; }
    bool IsNullable { get; }
    Type UnderlyingType { get; }
    IEnumerable<ITypeDescription> Parameters { get; }
}

public class TypeDescription(Type type, bool isNullable, Func<IEnumerable<ITypeDescription>> getParameters) : ITypeDescription
{
    public TypeDescription(NullabilityInfo nullability)
        : this(nullability.Type, nullability.ReadState is not NullabilityState.NotNull, () => nullability.GenericTypeArguments.Select(x => new TypeDescription(x)).ToList()) { }

    public Type Type => type;
    public bool IsNullable => isNullable;
    public Type UnderlyingType => Nullable.GetUnderlyingType(Type) ?? Type;

    private readonly Lazy<IEnumerable<ITypeDescription>> parameters = new(() => getParameters());
    public IEnumerable<ITypeDescription> Parameters => parameters.Value;
}