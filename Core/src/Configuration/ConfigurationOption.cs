namespace Markwardt;

public interface IConfigurationOption
{
    IConfigurationKey Key { get; }
    Type Type { get; }
    object? Value { get; }

    IObservable<object?> Changed { get; }

    void Reset();
}

public interface IConfigurationOption<T> : IConfigurationOption
{
    new ConfigurationKey<T> Key { get; }
    new T Value { get; }

    new IObservable<T> Changed { get; }

    bool Set(T value);
}

public class ConfigurationOption<T>(ConfigurationKey<T> key, T defaultValue, Func<T, bool>? isValid = null) : IConfigurationOption<T>
{
    private readonly T defaultValue = defaultValue;

    public T Value { get; private set; } = defaultValue;

    public ConfigurationKey<T> Key => key;
    public Type Type => typeof(T);

    private readonly Subject<T> changed = new();
    public IObservable<T> Changed => changed;

    object? IConfigurationOption.Value => Value;
    IObservable<object?> IConfigurationOption.Changed => Changed.Select(x => (object?)x);
    IConfigurationKey IConfigurationOption.Key => Key;

    public bool Set(T value)
    {
        if (isValid is not null && !isValid(value))
        {
            return false;
        }

        Value = value;
        changed.OnNext(value);
        return true;
    }

    public void Reset()
        => Set(defaultValue);
}