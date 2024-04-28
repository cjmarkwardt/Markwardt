namespace Markwardt;

public interface IGenerator<T>
{
    IGeneratable<T> Configure();
}

public static class GeneratorExtensions
{
    public static async ValueTask<T> Generate<T>(this IGenerator<T> generator)
        => await generator.Configure().Generate();
}

public abstract class Generator<T> : IGenerator<T>
{
    public IGeneratable<T> Configure()
        => new Generatable(Generate, CreateOptions());

    protected abstract IEnumerable<IConfigurationOption> CreateOptions();

    protected abstract ValueTask<T> Generate(IEnumerable<IConfigurationOption> options);

    private sealed class Generatable(Func<IEnumerable<IConfigurationOption>, ValueTask<T>> generate, IEnumerable<IConfigurationOption> options) : IGeneratable<T>
    {
        public IEnumerable<IConfigurationOption> Options { get; } = options;

        public async ValueTask<T> Generate()
            => await generate(Options);
    }
}