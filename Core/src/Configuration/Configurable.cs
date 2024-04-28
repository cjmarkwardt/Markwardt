namespace Markwardt;

public interface IConfigurable
{
    IEnumerable<IConfigurationOption> Options { get; }
}

public static class ConfigurableExtensions
{
    public static IConfigurationOption GetOption(this IEnumerable<IConfigurationOption> options, IConfigurationKey key)
        => options.FirstOrDefault(x => x.Key == key) ?? throw new InvalidOperationException($"Configuration option not found at {key.Section} -> {key.Name}");

    public static IConfigurationOption<T> GetOption<T>(this IEnumerable<IConfigurationOption> options, ConfigurationKey<T> key)
        => (IConfigurationOption<T>)options.GetOption((IConfigurationKey)key);

    public static T GetValue<T>(this IEnumerable<IConfigurationOption> options, ConfigurationKey<T> key)
        => options.GetOption(key).Value;
}