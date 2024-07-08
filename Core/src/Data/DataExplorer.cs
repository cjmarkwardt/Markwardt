namespace Markwardt;

public interface IDataExplorer
{
    IDataPointer GetStream(Stream data, int initialBlock);
}