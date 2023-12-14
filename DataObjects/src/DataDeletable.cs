namespace Markwardt;

public interface IDataDeletable
{
    bool IsDeleted { get; }

    void Delete();
}