namespace Markwardt;

public interface IConfigurationKey
{
    string Section { get; }
    string Name { get; }
    Type Type { get; }
}

public record ConfigurationKey<T>(string Section, string Name) : IConfigurationKey
{
    public Type Type => typeof(T);

    public IConfigurationOption<T> CreateOption(T defaultValue, Func<T, bool>? isValid = null)
        => new ConfigurationOption<T>(this, defaultValue, isValid);
}