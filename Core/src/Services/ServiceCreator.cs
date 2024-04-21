namespace Markwardt;

public interface IServiceCreator
{
    ValueTask<object> Create();
}