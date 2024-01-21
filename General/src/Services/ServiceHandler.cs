namespace Markwardt;

public interface IServiceHandler
{
    IServiceDescription? Get(object key);
}