namespace Markwardt;

public static class OptionExtensions
{
    public static bool TryGetValue<T>(this Option<T> option, [NotNullWhen(true)] out T value)
    {
        System.Text.Json.Nodes.JsonNode g;
        value = option.ValueOrDefault();
        return option.HasValue;
    }

    public static bool TryGetValue<T>(this Optional<T> option, [NotNullWhen(true)] out T? value)
        where T : notnull
    {
        value = option.ValueOrDefault();
        return option.HasValue;
    }

    public static Option<TValue> GetValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
        => dictionary.TryGetValue(key, out TValue? value) ? value.Some() : Option.None<TValue>();
}