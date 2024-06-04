namespace Markwardt;

public interface IStreamPackage
{
    IDisposable<Stream>? Open(string name);
}