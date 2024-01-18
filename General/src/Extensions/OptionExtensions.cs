namespace Markwardt;

public static class OptionExtensions
{
    public static bool TryGetValue<T>(this Option<T> option, [NotNullWhen(true)] out T value)
    {
        value = option.ValueOrDefault();
        return option.HasValue;
    }

    public static bool TryGetValue<T>(this Optional<T> option, [NotNullWhen(true)] out T? value)
        where T : notnull
    {
        value = option.ValueOrDefault();
        return option.HasValue;
    }
}