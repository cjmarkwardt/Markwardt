namespace Markwardt;

public interface IServiceConfiguration
{
    void Configure(object key, IServiceDescription description);
    void Clear(object key);
}