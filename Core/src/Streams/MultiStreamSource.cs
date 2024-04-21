namespace Markwardt;

public interface IMultiStreamSource
{
    IStreamSource Get(string id);
}