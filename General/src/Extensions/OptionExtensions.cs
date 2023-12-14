namespace Markwardt;

public static class OptionExtensions
{
    public static bool TryGetValue<T>(this Option<T> option, [NotNullWhen(true)] out T value)
    {
        value = option.ValueOrDefault();
        return option.HasValue;
    }
}