namespace Markwardt;

public interface IConvertibleObject
{
    bool TryConvert(Type type, out object? converted);
}

public static class ConvertibleObjectExtensions
{
    public static bool TryConvert(this object target, Type type, out object? converted)
    {
        if (target is IConvertibleObject convertible && convertible.TryConvert(type, out converted))
        {
            return true;
        }
        if (target.GetType().IsAssignableTo(type))
        {
            converted = target;
            return true;
        }
        else
        {
            converted = default;
            return false;
        }
    }

    public static bool TryConvert<T>(this object target, out T converted)
    {
        if (target.TryConvert(typeof(T), out object? rawConverted))
        {
            converted = (T)rawConverted!;
            return true;
        }
        else
        {
            converted = default!;
            return false;
        }
    }

    public static object? Convert(this object target, Type type)
        => target.TryConvert(type, out object? converted) ? converted : throw new InvalidOperationException($"Cannot convert {target.GetType()} to {type}");

    public static T Convert<T>(this object target)
        => (T)target.Convert(typeof(T))!;
}