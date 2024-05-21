namespace Markwardt;

public static class ObjectExtensions
{
    public static T CastTo<T>(this object? obj)
        => (T)obj!;

    public static T Act<T>(this T obj, Action<T> action)
    {
        action(obj);
        return obj;
    }
}