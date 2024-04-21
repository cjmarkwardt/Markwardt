namespace Markwardt;

public static class ObjectExtensions
{
    public static T CastTo<T>(this object? obj)
        => (T)obj!;
}