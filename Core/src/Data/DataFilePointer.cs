namespace Markwardt;

public interface IDataFilePointer : IDataPointer
{
    int Id { get; }

    new IDataFilePointer Copy();
    ValueTask Resize(int size);
    ValueTask Delete();
}