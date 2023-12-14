namespace Markwardt;

public interface IIdDataNotifier
{
    void MarkEntityDeleted(string id);
    void MarkEntityExpired(string id);
    void MarkIndexChanged(string index, string entityId);
}