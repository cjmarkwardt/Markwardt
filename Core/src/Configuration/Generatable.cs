namespace Markwardt;

public interface IGeneratable<T> : IConfigurable
{
    ValueTask<Failable<T>> Generate();
}

public interface IGeneratable<T, in TInput> : IConfigurable
{
    ValueTask<Failable<T>> Generate(TInput input);
}