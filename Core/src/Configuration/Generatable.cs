namespace Markwardt;

public interface IGeneratable<T> : IConfigurable
{
    ValueTask<T> Generate();
}