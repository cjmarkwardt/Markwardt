namespace Markwardt;

public interface IGenerator<T>
{
    IGeneratable<T> Configure();
}

public interface IGenerator<T, in TInput>
{
    IGeneratable<T, TInput> Configure();
}

public static class GeneratorExtensions
{
    public static async ValueTask<Failable<T>> Generate<T>(this IGenerator<T> generator)
        => await generator.Configure().Generate();
    
    public static async ValueTask<Failable<T>> Generate<T, TInput>(this IGenerator<T, TInput> generator, TInput input)
        => await generator.Configure().Generate(input);
}

public abstract class Generator<T> : IGenerator<T>
{
    public IGeneratable<T> Configure()
        => new Generatable(Generate, CreateOptions());

    protected abstract IEnumerable<IConfigurationOption> CreateOptions();

    protected abstract ValueTask<Failable<T>> Generate(IEnumerable<IConfigurationOption> options);

    private sealed class Generatable(Func<IEnumerable<IConfigurationOption>, ValueTask<Failable<T>>> generate, IEnumerable<IConfigurationOption> options) : IGeneratable<T>
    {
        public IEnumerable<IConfigurationOption> Options { get; } = options;

        public async ValueTask<Failable<T>> Generate()
            => await generate(Options);
    }
}

public abstract class Generator<T, TInput> : IGenerator<T, TInput>
{
    public IGeneratable<T, TInput> Configure()
        => new Generatable(Generate, CreateOptions());

    protected abstract IEnumerable<IConfigurationOption> CreateOptions();

    protected abstract ValueTask<Failable<T>> Generate(IEnumerable<IConfigurationOption> options);

    private sealed class Generatable(Func<IEnumerable<IConfigurationOption>, ValueTask<Failable<T>>> generate, IEnumerable<IConfigurationOption> options) : IGeneratable<T, TInput>
    {
        public IEnumerable<IConfigurationOption> Options { get; } = options;

        public async ValueTask<Failable<T>> Generate(TInput input)
            => await generate(Options);
    }
}